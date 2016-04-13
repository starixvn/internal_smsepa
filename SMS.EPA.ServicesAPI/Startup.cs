using Microsoft.Owin;

[assembly: OwinStartup(typeof(SMS.EPA.ServicesAPI.Startup))]

namespace SMS.EPA.ServicesAPI
{
    using Microsoft.Owin.Cors;
    using Owin;
    using System.Web.Http;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            ConfigureAuth(app);

            WebApiConfig.Register(config);
            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(config);
        }
    }
}
