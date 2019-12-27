namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Core.Model;
    using EPA.Model.API;
    using Fanex.Logging;
    using Model;

    public class EmployeeAPI : IEmployeeAPI
    {
        public async Task<DateTime> GetLastModifiedDateAsync(TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = await client.GetAsync(new Uri(ApplicationConfig.ServiceAPI + "Employee"));

                return await result.Content.ReadAsAsync<DateTime>();
            }
        }

        public async Task<CommandResult> Update(IEnumerable<Employee> employees, TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = await client.PostAsJsonAsync(ApplicationConfig.ServiceAPI + "Employee", employees.ToArray());

                if (result.StatusCode != HttpStatusCode.OK)
                {
                    LogService.Error(string.Format(
                        CultureInfo.InvariantCulture,
                        "Update Employee: \r\n StatusCode: {0} \r\n ReasonPhrase: {1}",
                        (int)result.StatusCode,
                        result.ReasonPhrase));
                }

                return new CommandResult { Code = (int)result.StatusCode };
            }
        }
    }
}