using SMS.EPA.ClientHRM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SMS.EPA.ClientHRM.ThirdPartyAPIs
{
    public class TeamAPI : ITeamAPI
    {
        public DateTime GetMaxModifiedDate(TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = client.GetAsync(ApplicationConfig.ServiceAPI + "Team").Result;
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    return result.Content.ReadAsAsync<DateTime>().Result;
                }
            }

            return DateTime.MinValue;
        }

        public CommandResult Update(IEnumerable<Team> teams, TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = client.PostAsJsonAsync<Team[]>(ApplicationConfig.ServiceAPI + "Team", teams.ToArray()).Result;

                return new CommandResult { Code = (int)result.StatusCode };
            }
        }
    }
}
