namespace SMS.EPA.ClientHrm.ThirdPartyAPIs
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using Newtonsoft.Json;
    using SMS.EPA.ClientHrm.Model;

    public class TokenAPI : ITokenAPI
    {
        public TokenAccess Request(ApplicationInfo info)
        {
            var token = new TokenAccess();
            using (var client = new HttpClient())
            {
                var form = new Dictionary<string, string> { { "grant_type", info.GrantType }, { "username", info.UserName }, { "password", info.Password } };
                var tokenResponse = client.PostAsync(ApplicationConfig.ServiceAPI + "token", new FormUrlEncodedContent(form)).Result;

                if (tokenResponse.StatusCode != HttpStatusCode.OK)
                {
                    throw new WebApiException(tokenResponse.StatusCode, tokenResponse.RequestMessage, JsonConvert.SerializeObject(form));
                }

                return tokenResponse.Content.ReadAsAsync<TokenAccess>(new[] { new JsonMediaTypeFormatter() }).Result;
            }
        }
    }
}