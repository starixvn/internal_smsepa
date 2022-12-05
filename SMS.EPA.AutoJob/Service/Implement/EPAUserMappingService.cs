namespace SMS.EPA.AutoJob.Service
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Jose;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using SMS.EPA.AutoJob.Criteria;
    using SMS.EPA.AutoJob.Models;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model.API;

    public class EPAUserMappingService : IEPAUserMappingService
    {
        private readonly IBaseService baseService;
        private readonly UCSettings ucSettings;
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(EPAUserMappingService));

        public EPAUserMappingService(
            IBaseService baseService,
            IOptions<UCSettings> ucSettings
        )
        {
            this.baseService = baseService;
            this.ucSettings = ucSettings.Value;
        }

        public async Task SyncDataUserMapping()
        {
            var lastSpecialUCDate = await GetLastSpecialUCDateAsync();
            var userMappings = await GetUserMapping(lastSpecialUCDate);
            await UpdateUserMapping(userMappings);
        }

        public async Task<DateTime?> GetLastSpecialUCDateAsync()
        {
            var criteria = new GetLastSpecialUCDateCriteria();
            var lastSpecialUCDate = await baseService.GetDataEntityAsync<DateTime?>(criteria);
            return lastSpecialUCDate;
        }

        public async Task<IEnumerable<UserMapping>> GetUserMapping(DateTime? lastSpecialUCDate)
        {
            var formatUCDate = lastSpecialUCDate.HasValue ? lastSpecialUCDate.Value.ToString(Constants.ModifiedDateTimeSendFormat) : null;

            var endPoint = string.Format(Constants.UrlGetLastUCModifiedDate, formatUCDate);
            try
            {
                using (var request = CreateRequest(endPoint, HttpMethod.Get, ucSettings))
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

        private static HttpRequestMessage CreateRequest(string endPoint, HttpMethod method, UCSettings ucSettings)
        {
            Uri url = new Uri($"{ucSettings.ServiceUrl}{endPoint}");
            var request = new HttpRequestMessage(method, url);
            request.Headers.Accept.Clear();
            string siteIdentity = GenerateSiteIdentity(ucSettings);
            request.Headers.Add("SiteIdentity", $"Bearer {siteIdentity}");

            return request;
        }

        private static string GenerateSiteIdentity(UCSettings ucSettings)
        {
            var data = new Dictionary<string, object>
            {
                { "SiteId", ucSettings.SiteId },
                { "AccessToken", ucSettings.AccessToken }
            };
            return JWT.Encode(data, Encoding.UTF8.GetBytes(ucSettings.AccessToken), JwsAlgorithm.HS256);
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
