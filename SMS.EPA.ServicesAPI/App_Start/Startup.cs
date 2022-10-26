namespace SMS.EPA.ServicesAPI
{
    using System;
    using System.Web.Http;
    using Fanex.Data;
    using Fanex.Data.SqlServer;
    using Microsoft.Owin;
    using Microsoft.Owin.Cors;
    using Microsoft.Owin.Security.OAuth;
    using Owin;
    using Providers;
    using Utilities;

    public class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            using (var config = new HttpConfiguration())
            {
                WebApiConfig.Register(config);
                app.UseCors(CorsOptions.AllowAll);
                app.UseWebApi(config);

                DbSettingProviderManager
                    .StartNewSession()
                    .WithSqlServer("~/App_Data/SpName.xml")
                    .Run();

                log4net.Config.XmlConfigurator.Configure();
            }
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