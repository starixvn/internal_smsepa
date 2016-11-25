namespace SMS.EPA.ClientHrm.ThirdPartyAPIs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using SMS.EPA.ClientHrm.Model;

    public class DisciplineAPI : IDisciplineAPI
    {
        public DateTime GetLastModifiedDate(TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = client.GetAsync(ApplicationConfig.ServiceAPI + "Discipline/GetLastModifiedDate").Result;
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    throw new WebApiException(result.StatusCode, result.RequestMessage);
                }

                return result.Content.ReadAsAsync<DateTime>().Result;
            }
        }

        public DateTime GetFromDateDiscipline(TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = client.GetAsync(ApplicationConfig.ServiceAPI + "Discipline/GetFromDateDiscipline").Result;
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    throw new WebApiException(result.StatusCode, result.RequestMessage);
                }

                return result.Content.ReadAsAsync<DateTime>().Result;
            }
        }

        public CommandResult Update(IEnumerable<Discipline> disciplines, TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = client.PostAsJsonAsync<Discipline[]>(ApplicationConfig.ServiceAPI + "Discipline", disciplines.ToArray()).Result;

                return new CommandResult { Code = (int)result.StatusCode };
            }
        }
    }
}