namespace SMS.EPA.AutoJob.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Jose;
    using Newtonsoft.Json;
    using SMS.EPA.AutoJob.Criteria;
    using SMS.EPA.AutoJob.Extensions.XML;
    using SMS.EPA.AutoJob.Models;

    public class UserMappingService : IUserMappingService
    {
        private readonly IBaseService baseService;
        private readonly UserControlSettings userControlSettings;
        private readonly IHttpClientFactory clientFactory;
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(UserMappingService));

        public UserMappingService(
            IHttpClientFactory clientFactory,
            IBaseService baseService,
            UserControlSettings userControlSettings)
        {
            this.clientFactory = clientFactory;
            this.baseService = baseService;
            this.userControlSettings = userControlSettings;
        }

        public async Task SyncUserMappingAsync()
        {
            var modifiedDate = await GetLastModifiedDateAsync();
            IEnumerable<UserMapping> userMappings = await GetUserMappingAsync(modifiedDate);
            await UpdateUserMappingAsync(userMappings);
        }

        public async Task<DateTime?> GetLastModifiedDateAsync()
        {
            var criteria = new GetLastModifiedDateUserMappingCriteria();
            return await baseService.GetDataEntityAsync<DateTime?>(criteria);
        }

        public async Task<IEnumerable<UserMapping>> GetUserMappingAsync(DateTime? lastModifiedDate)
        {
            var modifiedDate = lastModifiedDate.HasValue ? lastModifiedDate.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : null;

            var httpClient = clientFactory.CreateClient();
            var endPoint = string.Format("api/UserMapping/GetBySiteId?lastModifiedDate={0}", modifiedDate);
            try
            {
                using (var request = CreateRequest(endPoint, HttpMethod.Get))
                {
                    using (var response = await httpClient.SendAsync(request))
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

        public async Task<CommandResult> UpdateUserMappingAsync(IEnumerable<UserMapping> userMappings)
        {
            var criteria = new UpdateUserMappingCommand
            {
                UserMappingXml = userMappings != null && userMappings.Any() ? userMappings.ToXML().ToString() : string.Empty
            };

            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        private HttpRequestMessage CreateRequest(string endPoint, HttpMethod method)
        {
            var url = new Uri($"{userControlSettings.ServiceUrl}{endPoint}");
            var request = new HttpRequestMessage(method, url);
            request.Headers.Accept.Clear();
            string siteIdentity = GenerateSiteIdentity();
            request.Headers.Add("SiteIdentity", $"Bearer {siteIdentity}");

            return request;
        }

        private string GenerateSiteIdentity()
        {
            var data = new Dictionary<string, object>
            {
                { "SiteId", userControlSettings.SiteId },
                { "AccessToken", userControlSettings.AccessToken }
            };
            return JWT.Encode(data, Encoding.UTF8.GetBytes(userControlSettings.AccessToken), JwsAlgorithm.HS256);
        }
    }
}