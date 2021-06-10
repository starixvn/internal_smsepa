namespace SMS.EPA.WindowsService.ThirdPartyAPI
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Core.Model;
    using EPA.Model.API;
    using Model;
    using Newtonsoft.Json;

    public class DepartmentAPI : IDepartmentAPI
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(DepartmentAPI));
        private readonly IHttpClientFactory clientFactory;

        public DepartmentAPI(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public async Task<DateTime> GetLastModifiedDate(TokenAccess token)
        {
            var client = clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

            var result = await client.GetAsync(new Uri(ApplicationConfig.ServiceAPI + "Department"));

            return JsonConvert.DeserializeObject<DateTime>(await result.Content.ReadAsStringAsync());
        }

        public async Task<CommandResult> UpdateAsync(IEnumerable<Department> departments, TokenAccess token)
        {
            var client = clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

            var result = await client.PostAsJsonAsync(ApplicationConfig.ServiceAPI + "Department", departments.ToArray());

            if (result.StatusCode != HttpStatusCode.OK)
            {
                logger.Error(string.Format(
                    CultureInfo.InvariantCulture,
                    "Update Department: \r\n StatusCode: {0} \r\n ReasonPhrase: {1}",
                    (int)result.StatusCode,
                    result.ReasonPhrase));
            }

            return new CommandResult { Code = (int)result.StatusCode };
        }
    }
}