namespace SMS.EPA.ServicesAPI
{
    using Microsoft.Owin.Security.OAuth;
    using SMS.EPA.ServicesAPI.Filters;
    using System.Web.Http;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            config.Filters.Add(new IPHostValidationAttribute());
            //config.Filters.Add(new HttpsRequiredAttribute());

            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
