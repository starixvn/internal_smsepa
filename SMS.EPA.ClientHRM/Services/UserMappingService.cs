namespace SMS.EPA.ClientHrm.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Jose;
    using Newtonsoft.Json;
    using SMS.EPA.ClientHrm.Criteria;
    using SMS.EPA.ClientHRM.Criteria;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model.API;

    public class UserMappingService : IUserMappingService
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(UserMappingService));
        private readonly IBaseService baseService;
        private static readonly HttpClient httpClient = new HttpClient();

        public UserMappingService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<DateTime?> GetLastSyncUserDateAsync()
        {
            var criteria = new GetLastSyncUserDateCriteria();
            var lastSyncDate = await baseService.GetDataEntityAsync<DateTime?>(criteria);
            return lastSyncDate;
        }

        public async Task<IEnumerable<UserMapping>> GetUserMapping(string requestUrl, DateTime? lastSyncDate)
        {
            var endPoint = string.Format(requestUrl, lastSyncDate);
            try
            {
                using (var request = CreateRequest(endPoint, HttpMethod.Get))
                {
                    using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var responseData = await response.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<IEnumerable<UserMapping>>(responseData);

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
            catch (Exception ex)
            {
                logger.Error(ex.Message);

                return default;
            }
        }


        private static HttpRequestMessage CreateRequest(string endPoint, HttpMethod method)
        {
            Uri url = new Uri($"{ApplicationConfig.UserControlSettings_ServiceUrl}{endPoint}");
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
                { "SiteId", ApplicationConfig.UserControlSettings_SiteId },
                { "AccessToken", ApplicationConfig.UserControlSettings_AccessToken }
            };
            return JWT.Encode(data, Encoding.UTF8.GetBytes(ApplicationConfig.UserControlSettings_AccessToken), JwsAlgorithm.HS256);
        }

        public async Task<CommandResult> UpdateUserMapping(IEnumerable<UserMapping> userMappings)
        {
            var criteria = new UpdateUserMappingCriteria
            {
                UserMappings = userMappings
            };

            return await baseService.ExecuteNonQueryAsync(criteria);
        }
    }
}
