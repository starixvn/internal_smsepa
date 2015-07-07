// ----------------------------------------------------------------------
// <copyright file="WebApiConfig.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/2/2015 10:18:16 AM">Created.</change>
// </history>

namespace SMS.EPA.Site
{
    using System.Web.Http;
    using Newtonsoft.Json;

    /// <summary>
    /// The Web API config.
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Register web API routes.
        /// </summary>
        /// <param name="config">THe http configuration.</param>
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "CustomApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional });

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            var jsonFormatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            jsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
        }
    }
}
