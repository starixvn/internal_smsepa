namespace SMS.EPA.WindowsService.ThirdPartyAPI
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Core.Model;
    using EPA.Model.API;
    using Model;

    public class JobTitleAPI : IJobTitleAPI
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(JobTitleAPI));
        private readonly IHttpClientFactory clientFactory;

        public JobTitleAPI(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public async Task<CommandResult> UpdateAsync(IEnumerable<JobTitle> jobTitles, TokenAccess token)
        {
            var client = clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

            var result = await client.PostAsJsonAsync(ApplicationConfig.ServiceAPI + "JobTitle", jobTitles.ToArray());

            if (result.StatusCode != HttpStatusCode.OK)
            {
                logger.Error(string.Format(
                    CultureInfo.InvariantCulture,
                    "Update JobTitle: \r\n StatusCode: {0} \r\n ReasonPhrase: {1}",
                    (int)result.StatusCode,
                    result.ReasonPhrase));
            }

            return new CommandResult { Code = (int)result.StatusCode };
        }
    }
}