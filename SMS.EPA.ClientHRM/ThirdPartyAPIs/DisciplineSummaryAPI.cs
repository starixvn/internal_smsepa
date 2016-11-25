namespace SMS.EPA.ClientHrm.ThirdPartyAPIs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using SMS.EPA.ClientHrm.Model;

    public class DisciplineSummaryAPI : IDisciplineSummaryAPI
    {
        public DateTime GetLastModifiedDate(TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = client.GetAsync(ApplicationConfig.ServiceAPI + "DisciplineSummary/GetLastModifiedDate").Result;
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    throw new WebApiException(result.StatusCode, result.RequestMessage);
                }

                return result.Content.ReadAsAsync<DateTime>().Result;
            }
        }

        public CommandResult Update(IEnumerable<DisciplineSummary> disciplines, TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = client.PostAsJsonAsync<DisciplineSummary[]>(ApplicationConfig.ServiceAPI + "DisciplineSummary", disciplines.ToArray()).Result;

                return new CommandResult { Code = (int)result.StatusCode };
            }
        }
    }
}