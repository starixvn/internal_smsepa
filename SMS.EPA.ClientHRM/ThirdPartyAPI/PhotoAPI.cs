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
    using Model;

    public class PhotoAPI : IPhotoAPI
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(PhotoAPI));

        public async Task<DateTime> GetLastModifiedDateAsync(TokenAccess token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

                var result = await client.GetAsync(new Uri(ApplicationConfig.ServiceAPI + "Photo"));

                return await result.Content.ReadAsAsync<DateTime>();
            }
        }

        public async Task<CommandResult> Update(IEnumerable<Photo> photos, TokenAccess token)
        {
            using (var client = new HttpClient())
            {
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
}