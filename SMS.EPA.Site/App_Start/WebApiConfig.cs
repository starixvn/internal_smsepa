namespace SMS.EPA.Site
{
    using Core.Attribute;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System.Web.Http;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            if (config != null)
            {
                config.Routes.MapHttpRoute(
                    name: "CustomApi",
                    routeTemplate: "api/{controller}/{action}/{id}",
                    defaults: new { id = RouteParameter.Optional });

                config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional });

                config.Filters.Add(new ExceptionHandlingAttribute());
            }

            var jsonFormatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            jsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
            ((DefaultContractResolver)jsonFormatter.SerializerSettings.ContractResolver).IgnoreSerializableAttribute = true;
        }
    }
}