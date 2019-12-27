namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using Core.Model;
    using EPA.Model.API;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net;
    using Fanex.Logging;
    using System.Globalization;
    using System.Threading.Tasks;

    public class EmployeeDisciplinePermissionAPI : IEmployeeSupervisorAPI
    {
        public async Task<DateTime> GetLastModifiedDateAsync(TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = await client.GetAsync(new Uri(ApplicationConfig.ServiceAPI + "EmployeeDisciplinePermission"));

                return await result.Content.ReadAsAsync<DateTime>();
            }
        }

        public async Task<CommandResult> UpdateAsync(IEnumerable<EmployeeDisciplinePermission> employeeSupervisors, TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
                var targetURL = ApplicationConfig.ServiceAPI + "EmployeeDisciplinePermission";
                var result = await client.PostAsJsonAsync(targetURL, employeeSupervisors.ToArray());

                if (result.StatusCode != HttpStatusCode.OK)
                {
                    LogService.Error(string.Format(
                        CultureInfo.InvariantCulture,
                        "Update EmployeeDisciplinePermission: \r\n StatusCode: {0} \r\n ReasonPhrase: {1}",
                        (int)result.StatusCode,
                        result.ReasonPhrase));
                }

                return new CommandResult { Code = (int)result.StatusCode };
            }
        }
    }
}