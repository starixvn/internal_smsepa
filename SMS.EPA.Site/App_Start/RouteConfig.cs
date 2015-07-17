// ----------------------------------------------------------------------
// <copyright file="RouteConfig.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/2/2015 10:15:42 AM">Created.</change>
// </history>

namespace SMS.EPA.Site
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// Route config.
    /// </summary>
    public static class RouteConfig
    {
        /// <summary>
        /// Register routes.
        /// </summary>
        /// <param name="routes">The routes.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "Login",
               url: "Auth/Login",
               defaults: new { controller = "Auth", action = "Login" });

            var homeRouteUrls = new List<string>
            {
                "Employee/Index",
                "KPI/Index",
                "KPIGroup/Index",
                "Role/Index",
                "Home/AccessDenied",
                "TargetTemplate/Index",
                "TargetTemplate/Create",
                "TargetTemplate/Update/{id}",
                "TargetTemplate/Detail/{id}",
                "Flow/Index"
            };

            homeRouteUrls.ForEach(url => routes.MapRoute(
                name: url,
                url: url,
                defaults: new
                {
                    controller = "Home",
                    action = "Index"
                }));

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}