namespace SMS.EPA.ServicesAPI
{
    using Microsoft.Owin.Security.OAuth;
    using SMS.EPA.ServicesAPI.Filters;
    using System.Web.Http;
    using System.Web.Http.ExceptionHandling;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            config.Filters.Add(new IPHostValidationAttribute());
            config.Filters.Add(new HttpsRequiredAttribute());
            config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());

            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { action = RouteParameter.Optional, id = RouteParameter.Optional });
        }
    }
}
