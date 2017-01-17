namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using SMS.EPA.ClientHrm.Model;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    public class TokenAPI : ITokenAPI
    {
        public TokenAccess Request(ApplicationInfo info)
        {
            using (var client = new HttpClient())
            {
                var form = new Dictionary<string, string> { { "grant_type", info.GrantType }, { "username", info.UserName }, { "password", info.Password } };
                using (var encodedContent = new FormUrlEncodedContent(form))
                {
                    var tokenResponse = client.PostAsync(new Uri(ApplicationConfig.ServiceAPI + "token"), encodedContent).Result;

                    return tokenResponse.Content.ReadAsAsync<TokenAccess>(new[] { new JsonMediaTypeFormatter() }).Result;
                }
            }
        }
    }
}