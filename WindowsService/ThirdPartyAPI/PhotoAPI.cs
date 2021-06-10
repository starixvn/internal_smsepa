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

    public class PhotoAPI : IPhotoAPI
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(PhotoAPI));
        private readonly IHttpClientFactory clientFactory;

        public PhotoAPI(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public async Task<DateTime> GetLastModifiedDateAsync(TokenAccess token)
        {
            var client = clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

            var result = await client.GetAsync(new Uri(ApplicationConfig.ServiceAPI + "Photo"));

            return JsonConvert.DeserializeObject<DateTime>(await result.Content.ReadAsStringAsync());
        }

        public async Task<CommandResult> Update(IEnumerable<Photo> photos, TokenAccess token)
        {
            var client = clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

            var result = await client.PostAsJsonAsync(ApplicationConfig.ServiceAPI + "Photo", photos.ToArray());

            if (result.StatusCode != HttpStatusCode.OK)
            {
                logger.Error(string.Format(
                    CultureInfo.InvariantCulture,
                    "Update Photo: \r\n StatusCode: {0} \r\n ReasonPhrase: {1}",
                    (int)result.StatusCode,
                    result.ReasonPhrase));
            }

            return new CommandResult { Code = (int)result.StatusCode };
        }
    }
}