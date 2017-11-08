namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Core.Model;
    using EPA.Model.API;
    using Fanex.Logging;
    using Model;

    public class DisciplineSummaryAPI : IDisciplineSummaryAPI
    {
        public DateTime GetLastModifiedDate(TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = client.GetAsync(new Uri(ApplicationConfig.ServiceAPI + "DisciplineSummary/GetLastModifiedDate")).Result;

                return result.Content.ReadAsAsync<DateTime>().Result;
            }
        }

        public CommandResult Update(IEnumerable<DisciplineSummary> disciplines, TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = client.PostAsJsonAsync(ApplicationConfig.ServiceAPI + "DisciplineSummary", disciplines.ToArray()).Result;

                if (result.StatusCode != HttpStatusCode.OK)
                {
                    LogService.Error(string.Format(
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