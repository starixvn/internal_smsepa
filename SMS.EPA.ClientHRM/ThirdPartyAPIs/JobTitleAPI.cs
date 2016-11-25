namespace SMS.EPA.ClientHrm.ThirdPartyAPIs
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using SMS.EPA.ClientHrm.Model;

    public class JobTitleAPI : IJobTitleAPI
    {
        public CommandResult Update(IEnumerable<JobTitle> jobTitles, TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = client.PostAsJsonAsync<JobTitle[]>(ApplicationConfig.ServiceAPI + "JobTitle", jobTitles.ToArray()).Result;

                return new CommandResult { Code = (int)result.StatusCode };
            }
        }
    }
}