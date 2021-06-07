namespace SMS.EPA.WindowsService.ThirdPartyAPI
{
    using System;
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
    using Newtonsoft.Json;

    public class DisciplineSummaryAPI : IDisciplineSummaryAPI
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(DisciplineSummaryAPI));

        public async Task<DateTime> GetLastModifiedDateAsync(TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = await client.GetAsync(new Uri(ApplicationConfig.ServiceAPI + "DisciplineSummary/GetLastModifiedDate"));

                return JsonConvert.DeserializeObject<DateTime>(await result.Content.ReadAsStringAsync());
            }
        }

        public async Task<CommandResult> UpdateAsync(IEnumerable<DisciplineSummary> disciplines, TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = await client.PostAsJsonAsync(ApplicationConfig.ServiceAPI + "DisciplineSummary", disciplines.ToArray());

                if (result.StatusCode != HttpStatusCode.OK)
                {
                    logger.Error(string.Format(
                        CultureInfo.InvariantCulture,
                        "Update Discipline: \r\n StatusCode: {0} \r\n ReasonPhrase: {1}",
                        (int)result.StatusCode,
                        result.ReasonPhrase));
                }

                return new CommandResult { Code = (int)result.StatusCode };
            }
        }
    }
}