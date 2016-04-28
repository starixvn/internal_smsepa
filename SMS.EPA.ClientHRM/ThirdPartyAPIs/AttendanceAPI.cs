namespace SMS.EPA.ClientHRM.ThirdPartyAPIs
{
    using SMS.EPA.ClientHRM.Model;
    using SMS.EPA.ServicesAPI.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public class AttendanceAPI : IAttendanceAPI
    {
        public DateTime GetMaxModifiedDate(TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = client.GetAsync(ApplicationConfig.ServiceAPI + "EmployeeDiscipline/GetMaxModifiedDate").Result;
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    return result.Content.ReadAsAsync<DateTime>().Result;
                }
            }

            return DateTime.MinValue;
        }

        public DateTime GetFromDateAttendanceViolation(TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = client.GetAsync(ApplicationConfig.ServiceAPI + "EmployeeDiscipline/GetFromDateAttendanceViolation").Result;
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    return result.Content.ReadAsAsync<DateTime>().Result;
                }
            }

            return DateTime.MinValue;
        }

        public CommandResult Update(IEnumerable<AttendanceViolation> employees, TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = client.PostAsJsonAsync<AttendanceViolation[]>(ApplicationConfig.ServiceAPI + "EmployeeDiscipline", employees.ToArray()).Result;

                return new CommandResult { Code = (int)result.StatusCode };
            }
        }
    }
}
