namespace SMS.EPA.ServicesAPI
{
    using Fanex.Data.Configuration;
    using Microsoft.Owin;
    using Microsoft.Owin.Cors;
    using Microsoft.Owin.Security.OAuth;
    using Owin;
    using SMS.EPA.ServicesAPI.Providers;
    using SMS.EPA.ServicesAPI.Utilities;
    using System;
    using System.Web.Http;

    public class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            var config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(config);

            DbSettingProviderManager.Current.Start();
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(AppConfig.TokenTimeOut),
                AllowInsecureHttp = false
            };
            app.UseOAuthBearerTokens(OAuthOptions);
        }
    }
}
