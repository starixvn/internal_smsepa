namespace SMS.EPA.ServicesAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
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

        public EmployeeController() : this(null, null)
        {
        }

        public EmployeeController(IServiceAPI service, IBaseService baseService)
        {
            _service = service ?? new ServiceAPI();
            this.baseService = baseService ?? new BaseService();
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
            foreach(Employee employee in employees)
            {
                string endPoint;
                var data = new Dictionary<string, object>
                {
                    { "code", employee.Code },
                    { "nickname", employee.NickName },
                    { "fullName", employee.FullName },
                    { "oldPassword", employee.Password },
                    { "email", employee.Email },
                    { "IsActive", employee.IsEnabled },
                    { "IsDeleted", employee.IsDeleted }
                };
                if (employee.UserId == 0)
                {
                    endPoint = "api/syncData/createUser";
                    var commandResult = await SendAsync<CommandResult>(endPoint, data);
                    await baseService.ExecuteNonQueryAsync(new UpdateUserIdCommand
                    {
                        EmployeeId = employee.EmployeeId,
                        UserId = commandResult.ObjectId
                    });
                }
                else
                {
                    endPoint = "api/syncData/updateUser";
                    data.Add("userId", employee.UserId);
                    await SendAsync<CommandResult>(endPoint, data);
                }
            }
        }

        private async Task<T> SendAsync<T>(string endPoint, IDictionary<string, object> data)
        {
            using (var request = CreateRequest(endPoint, HttpMethod.Post))
            {
                if (data != null)
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
                        var result = JsonConvert.DeserializeObject<T>(responseData);

                        return result;
                    }
                    else
                    {
                        var message = await response.Content.ReadAsStringAsync();
                        logger.Error(string.Format("StatusCode: {0}\r\nMessage: {1}", response.StatusCode, message));

                        return default;
                    }
                }
            }
        }

        private static HttpRequestMessage CreateRequest(string endPoint, HttpMethod method)
        {
            Uri url = new Uri($"{AppConfig.UserControlSettings_ServiceUrl}{endPoint}");
            var request = new HttpRequestMessage(method, url);
            request.Headers.Accept.Clear();
            string siteIdentity = GenerateSiteIdentity();
            request.Headers.Add("SiteIdentity", $"Bearer {siteIdentity}");

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