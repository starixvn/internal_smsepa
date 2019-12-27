namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading.Tasks;

    public class TokenAPI : ITokenAPI
    {
        public async Task<TokenAccess> RequestAsync(ApplicationInfo info)
        {
            using (var client = new HttpClient())
            {
                var form = new Dictionary<string, string> { { "grant_type", info.GrantType }, { "username", info.UserName }, { "password", info.Password } };
                using (var encodedContent = new FormUrlEncodedContent(form))
                {
                    var tokenResponse = await client.PostAsync(new Uri(ApplicationConfig.ServiceAPI + "token"), encodedContent);

                    return await tokenResponse.Content.ReadAsAsync<TokenAccess>(new[] { new JsonMediaTypeFormatter() });
                }
            }
        }
    }
}