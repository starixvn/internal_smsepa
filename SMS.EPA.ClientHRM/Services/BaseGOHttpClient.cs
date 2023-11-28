namespace SMS.EPA.ClientHrm.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models.Auth;
    using HRM.UserControl.Client.Core.Services;
    using Jose;
    using Newtonsoft.Json;

    public class BaseGOHttpClient
    {
        protected JwtToken _jwttoken = new JwtToken { ValidTo = DateTimeOffset.UtcNow.AddDays(-1).DateTime };
        private readonly ICryptoService cryptoService;
        private static readonly HttpClient httpClient = new HttpClient();

        public BaseGOHttpClient(ICryptoService cryptoService)
        {
            this.cryptoService = cryptoService;
        }

        protected async Task GetTokenAsync()
        {
            if (_jwttoken.IsExpired)
            {
                var token = cryptoService.GenerateRijndaelKeyIV();
                string[] keyiv = token.Split(':');
                var rijndaelKey = keyiv[0];
                var rijndaelIV = keyiv[1];
                var loginParam = new LoginParam
                {
                    Username = cryptoService.Encrypt(ApplicationConfig.UserControl_Username, rijndaelKey, rijndaelIV),
                    Password = cryptoService.Encrypt(ApplicationConfig.UserControl_Password, rijndaelKey, rijndaelIV),
                    Token = token
                };

                var loginResult = await PostAsync<LoginParam, LoginResult>("api/auth/login", loginParam);

                if (loginResult.IsSuccessfull)
                {
                    _jwttoken = loginResult.JwtToken;
                }
            }
        }

        protected static async Task<T> GetAsync<T>(string requestUrl, string jwtToken = null)
        {
            ValidateEndPoint(requestUrl);
            using (var request = CreateRequest(requestUrl, HttpMethod.Get, jwtToken))
            {
                using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None))
                {
                    ThrowExceptionWhenNotSuccess(response);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<T>(responseData);

                        return result;
                    }

                    return default(T);
                }
            }
        }

        protected static async Task<TOutput> PostAsync<TInput, TOutput>(string requestUrl, TInput data, string jwtToken = null)
        {
            ValidateEndPoint(requestUrl);
            using (var request = CreateRequest(requestUrl, HttpMethod.Post, jwtToken))
            {
                var encodedData = JsonConvert.SerializeObject(data);
                var content = new StringContent(encodedData, Encoding.UTF8, "application/json");
                request.Content = content;
                using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None))
                {
                    ThrowExceptionWhenNotSuccess(response);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<TOutput>(responseData);

                        return result;
                    }

                    return default(TOutput);
                }
            }
        }

        private static void ValidateEndPoint(string endPoint)
        {
            if (string.IsNullOrWhiteSpace(endPoint))
            {
                throw new ArgumentNullException(nameof(endPoint));
            }
        }

        private static HttpRequestMessage CreateRequest(string endPoint, HttpMethod method, string jwtToken = null)
        {
            var url = new Uri($"{ApplicationConfig.UserControlSettings_ServiceUrl}{endPoint}");
            var request = new HttpRequestMessage(method, url);
            request.Headers.Accept.Clear();
            string siteIdentity = GenerateSiteIdentity();
            request.Headers.Add("SiteIdentity", $"Bearer {siteIdentity}");
            if (!string.IsNullOrWhiteSpace(jwtToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            }

            return request;
        }

        private static string GenerateSiteIdentity()
        {
            var data = new Dictionary<string, object>
            {
                { "SiteId", ApplicationConfig.UserControlSettings_SiteId },
                { "AccessToken", ApplicationConfig.UserControlSettings_AccessToken }
            };
            return JWT.Encode(data, Encoding.UTF8.GetBytes(ApplicationConfig.UserControlSettings_AccessToken), JwsAlgorithm.HS256);
        }

        private static void ThrowExceptionWhenNotSuccess(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return;
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }

#pragma warning disable S4462 // Calls to "async" methods should not be blocking
                var message = response.Content.ReadAsStringAsync().Result;
#pragma warning restore S4462 // Calls to "async" methods should not be blocking
                var exception = new Exception(message);
                throw exception;
            }
        }
    }
}