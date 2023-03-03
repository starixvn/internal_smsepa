namespace SMS.EPA.ServicesAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using HRM.UserControl.Client.Core.Models.Auth;
    using HRM.UserControl.Client.Core.Services;
    using Jose;
    using Newtonsoft.Json;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model.API;
    using SMS.EPA.ServicesAPI.Criteria.Employee;
    using SMS.EPA.ServicesAPI.Services;
    using SMS.EPA.ServicesAPI.Utilities;

    [Authorize]
    public class EmployeeController : ApiController
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(EmployeeController));
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly IServiceAPI _service;
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;

        public EmployeeController() : this(null, null, null)
        {
        }

        public EmployeeController(
            IServiceAPI service,
            IBaseService baseService,
            ICryptoService cryptoService)
        {
            _service = service ?? new ServiceAPI();
            this.baseService = baseService ?? new BaseService();
            this.cryptoService = cryptoService ?? new CryptoService();
        }

        public async Task<IHttpActionResult> GetLastModifiedDate()
        {
            var criteria = new GetLastModifiedDateForEmployeeCriteria();
            DateTime fromDate = await _service.GetLastModifiedDate(criteria);

            return Ok(fromDate);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Update(Employee[] employees)
        {
            var criteria = new UpdateEmployeeCriteria
            {
                Employees = employees
            };
            var result = await baseService.GetDataAsync<Employee>(criteria);
            if (criteria.ErrCode > 0)
            {
                logger.Info(string.Format("Error: {0}\r\nEmployees: {1}", criteria.ErrMsg, JsonConvert.SerializeObject(employees)));
            }
            else
            {
                await SyncDataToUserControl(result);
            }

            return criteria.ErrCode == 0 ? StatusCode(HttpStatusCode.OK) : StatusCode(HttpStatusCode.NotModified);
        }

        private async Task SyncDataToUserControl(IEnumerable<Employee> employees)
        {
            LoginResult loginResult = await LoginAsync();
            if (loginResult.IsSuccessfull)
            {
                foreach (Employee employee in employees)
                {
                    string endPoint;
                    var username = !string.IsNullOrWhiteSpace(employee.DomainUsername) ? employee.DomainUsername.Replace("sms\\", string.Empty) : string.Empty;
                    var data = new Dictionary<string, object>
                    {
                        { "refId", employee.EmployeeId },
                        { "code", employee.Code },
                        { "username", username },
                        { "nickname", employee.NickName },
                        { "fullName", employee.FullName },
                        { "email", employee.Email },
                        { "IsActive", employee.IsEnabled },
                        { "IsDeleted", employee.IsDeleted }
                    };
                    if (employee.UserId.HasValue && employee.UserId.Value > 0)
                    {
                        endPoint = "api/user/update";
                        data.Add("userId", employee.UserId);
                        data.Add("OldPassword", employee.Password);
                        await SendAsync<Dictionary<string, object>, CommandResult>(endPoint, data, loginResult.JwtToken.Token);
                    }
                    else
                    {
                        endPoint = "api/user/create";
                        data.Add("Password", employee.Password);
                        var commandResult = await SendAsync<Dictionary<string, object>, CommandResult>(endPoint, data, loginResult.JwtToken.Token);
                        await baseService.ExecuteNonQueryAsync(new UpdateUserIdCommand
                        {
                            EmployeeId = employee.EmployeeId,
                            UserId = commandResult.ObjectId
                        });
                    }
                }
            }
        }

        private async Task<LoginResult> LoginAsync()
        {
            const string LoginPath = "api/auth/login";
            var token = cryptoService.GenerateRijndaelKeyIV();
            string[] keyiv = token.Split(':');
            var rijndaelKey = keyiv[0];
            var rijndaelIV = keyiv[1];
            var loginParam = new LoginParam
            {
                Username = cryptoService.Encrypt(AppConfig.UserControl_Username, rijndaelKey, rijndaelIV),
                Password = cryptoService.Encrypt(AppConfig.UserControl_Password, rijndaelKey, rijndaelIV),
                Token = token
            };

            return await SendAsync<LoginParam, LoginResult>(LoginPath, loginParam);
        }

        private async Task<TOutput> SendAsync<TInput, TOutput>(string endPoint, TInput data, string jwttoken = null)
        {
            try
            {
                using (var request = CreateRequest(endPoint, HttpMethod.Post, jwttoken))
                {
                    if (!object.Equals(data, default(TInput)))
                    {
                        var encodedData = JsonConvert.SerializeObject(data);
                        var content = new StringContent(encodedData, Encoding.UTF8, "application/json");
                        request.Content = content;
                    }
                    using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var responseData = await response.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<TOutput>(responseData);

                            return result;
                        }
                        else
                        {
                            var message = await response.Content.ReadAsStringAsync();
                            logger.Error(string.Format("StatusCode: {0}\r\nMessage: {1}", response.StatusCode, message));

                            return default(TOutput);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);

                return default(TOutput);
            }
        }

        private static HttpRequestMessage CreateRequest(string endPoint, HttpMethod method, string jwttoken = null)
        {
            Uri url = new Uri($"{AppConfig.UserControlSettings_ServiceUrl}{endPoint}");
            var request = new HttpRequestMessage(method, url);
            request.Headers.Accept.Clear();
            string siteIdentity = GenerateSiteIdentity();
            request.Headers.Add("SiteIdentity", $"Bearer {siteIdentity}");
            if (jwttoken != null && !string.IsNullOrWhiteSpace(jwttoken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwttoken);
            }

            return request;
        }

        private static string GenerateSiteIdentity()
        {
            var data = new Dictionary<string, object>
            {
                { "SiteId", AppConfig.UserControlSettings_SiteId },
                { "AccessToken", AppConfig.UserControlSettings_AccessToken }
            };
            return JWT.Encode(data, Encoding.UTF8.GetBytes(AppConfig.UserControlSettings_AccessToken), JwsAlgorithm.HS256);
        }
    }
}