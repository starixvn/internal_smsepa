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
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class RouteConfig
    {
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
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
                "Flow/Index",
                "Flow/Create",
                "Flow/Update/{id}",
                "TargetEmployeeSetting/Index",
                "Department/Index",
                "SelfEvaluation/Index",
                "SelfEvaluation/Detail/{id}",
                "Evaluator/Index",
                "Evaluator/Detail/{id}",
                "Reviewer/Index",
                "Reviewer/Detail/{id}",
                "Management/Index",
                "Management/Detail/{id}",
                "EPA/Detail/{id}",
                "OldEPA/Convert",
                "Period/Index",
                "Period/Update/{id}",
                "Home",
                "Discipline/Index",
                "RatingByKPI/Index",
                "Discipline/Detail/{id}",
                "Discipline/Detail",
                "Report/EPASummaryReport",
                "Report/EPADetailReport",
                "EPAHistory/Index",
                "MonthlyPeriod/Index",
                "MonthlyPeriod/Update/{id}",
                "EPA/SemiDetail/{id}",
                "Reviewer/SemiDetail/{id}",
                "Management/SemiDetail/{id}",
                "EPA/ResetScore/{id}",
                "ViewEPAByKPI/Index",
                "error"
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