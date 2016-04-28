namespace SMS.EPA.ClientHRM.ThirdPartyAPIs
{
    using SMS.EPA.ClientHRM.Model;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    public class TokenAPI : ITokenAPI
    {
        public TokenAccess Request(ApplicationInfo info)
        {
            var token = new TokenAccess();
            using (var client = new HttpClient())
            {
                var form = new Dictionary<string, string> { { "grant_type", info.GrantType }, { "username", info.UserName }, { "password", info.Password } };
                var tokenResponse = client.PostAsync(ApplicationConfig.ServiceAPI + "token", new FormUrlEncodedContent(form)).Result;

                if (tokenResponse.StatusCode == HttpStatusCode.OK)
                {
                    token = tokenResponse.Content.ReadAsAsync<TokenAccess>(new[] { new JsonMediaTypeFormatter() }).Result;
                }
            }

            return token;
        }
    }
}
