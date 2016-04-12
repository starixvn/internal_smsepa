namespace SMS.EPA.ServiceAPI.ClientConsole
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;

    public class Program
    {
        static void Main(string[] args)
        {
            try 
            {
                using (var client = new HttpClient())
                {
                    var token = new Token();
                    var form = new Dictionary<string, string>  
               {  
                   {"grant_type", "password"},
                   {"username", "app~1234~APP"},  
                   {"password", "app~1234~APP"},
               };
                    Console.WriteLine("1. Request token............");
                    var tokenResponse = client.PostAsync("http://localhost:63238/token", new FormUrlEncodedContent(form)).Result;

                    Console.WriteLine("Respone code: " + tokenResponse.StatusCode);
                    if (tokenResponse.StatusCode == HttpStatusCode.OK)
                    {
                        token = tokenResponse.Content.ReadAsAsync<Token>(new[] { new JsonMediaTypeFormatter() }).Result;

                        Console.WriteLine("Token:" + token.AccessToken);
                        Console.WriteLine("Expires In:" + token.ExpiresIn);

                        Console.WriteLine("2. Update department............");
                        var departments = new Dictionary<string, string>();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                        var result = client.PostAsync("http://localhost:63238/Department/Insert", new FormUrlEncodedContent(departments)).Result;

                        Console.WriteLine("Respone code: " + result.StatusCode);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadLine();
        }
    }
}
