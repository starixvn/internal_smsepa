using SMS.EPA.ClientHRM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SMS.EPA.ClientHRM.ThirdPartyAPI
{
    public class SupervisorAPI : ISupervisorAPI
    {
        public DateTime GetMaxModifiedDateForEmployee(TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = client.GetAsync(ApplicationConfig.ServiceAPI + "Permission/GetMaxModifiedDateForEmployeeDiscipline").Result;
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    return result.Content.ReadAsAsync<DateTime>().Result;
                }
            }

            return DateTime.MinValue;
        }

        public CommandResult UpdateEmployees(IEnumerable<EmployeeSupervisor> employeeSupervisors, TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
                var targetURL = ApplicationConfig.ServiceAPI + "Permission/UpdateEmployeeDiscipline";
                var result = client.PostAsJsonAsync<EmployeeSupervisor[]>(targetURL, employeeSupervisors.ToArray()).Result;

                return new CommandResult { Code = (int)result.StatusCode };
            }
        }

        public DateTime GetMaxModifiedDateForGroup(TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = client.GetAsync(ApplicationConfig.ServiceAPI + "Permission/GetMaxModifiedDateForTeamDiscipline").Result;
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    return result.Content.ReadAsAsync<DateTime>().Result;
                }
            }

            return DateTime.MinValue;
        }

        public CommandResult UpdateGroups(IEnumerable<GroupSupervisor> groupSupervisors, TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = client.PostAsJsonAsync<GroupSupervisor[]>(ApplicationConfig.ServiceAPI + "Permission/UpdateTeamDiscipline", groupSupervisors.ToArray()).Result;

                return new CommandResult { Code = (int)result.StatusCode };
            }
        }
    }
}
