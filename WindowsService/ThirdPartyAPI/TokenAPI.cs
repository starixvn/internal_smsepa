namespace SMS.EPA.WindowsService.ThirdPartyAPI
{
    using Model;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class TokenAPI : ITokenAPI
    {
        private readonly IHttpClientFactory clientFactory;

        public TokenAPI(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public async Task<TokenAccess> RequestAsync(ApplicationInfo info)
        {
            var client = clientFactory.CreateClient();
            var form = new Dictionary<string, string> { { "grant_type", info.GrantType }, { "username", info.UserName }, { "password", info.Password } };
            using (var encodedContent = new FormUrlEncodedContent(form))
            {
                var tokenResponse = await client.PostAsync(new Uri(ApplicationConfig.ServiceAPI + "token"), encodedContent);

                return JsonConvert.DeserializeObject<TokenAccess>(await tokenResponse.Content.ReadAsStringAsync());
            }
        }
    }
}