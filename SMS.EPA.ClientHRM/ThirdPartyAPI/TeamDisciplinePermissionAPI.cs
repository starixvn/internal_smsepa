namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using Core.Model;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public class TeamDisciplinePermissionAPI : ITeamSupervisorAPI
    {
        public DateTime GetLastModifiedDate(TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = client.GetAsync(new Uri(ApplicationConfig.ServiceAPI + "TeamDisciplinePermission")).Result;

                return result.Content.ReadAsAsync<DateTime>().Result;
            }
        }

        public CommandResult Update(IEnumerable<GroupDisciplinePermission> groupSupervisors, TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = client.PostAsJsonAsync(ApplicationConfig.ServiceAPI + "TeamDisciplinePermission", groupSupervisors.ToArray()).Result;

                return new CommandResult { Code = (int)result.StatusCode };
            }
        }
    }
}