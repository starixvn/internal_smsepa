namespace SMS.EPA.ServicesAPI
{
    using Microsoft.Owin;
    using Microsoft.Owin.Security.OAuth;
    using Owin;
    using SMS.EPA.ServicesAPI.Providers;
    using System;

    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(10)
            };
            app.UseOAuthBearerTokens(OAuthOptions);
        }
    }
}
