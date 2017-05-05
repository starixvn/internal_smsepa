namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using Core.Model;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public class EmployeeAPI : IEmployeeAPI
    {
        public DateTime GetLastModifiedDate(TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = client.GetAsync(new Uri(ApplicationConfig.ServiceAPI + "Employee")).Result;

                return result.Content.ReadAsAsync<DateTime>().Result;
            }
        }

        public CommandResult Update(IEnumerable<Employee> employees, TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = client.PostAsJsonAsync(ApplicationConfig.ServiceAPI + "Employee", employees.ToArray()).Result;

                return new CommandResult { Code = (int)result.StatusCode };
            }
        }
    }
}