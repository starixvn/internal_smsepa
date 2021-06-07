namespace SMS.EPA.ServicesAPI
{
    using System;
    using System.Web.Http;
    using Fanex.Data;
    using Fanex.Data.OldConnectionStringProvider;
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
                .Use(new OldConnectionStringProvider())
                .UseDefaultDbSettingProvider(
                    resourcePath: "~/App_Data",
                    enableWatching: true)
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
                AllowInsecureHttp = true
            };
            app.UseOAuthBearerTokens(OAuthOptions);
        }
    }
}