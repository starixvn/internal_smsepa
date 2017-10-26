namespace SMS.EPA.Site
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class RouteConfig
    {
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed.")]
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "Login",
               url: "Auth/Login",
               defaults: new { controller = "Auth", action = "Login" });

            var homeRouteUrls = new List<string>
            {
                "Home",
                "Home/AccessDenied",
                "Home/InternalError",
                "Home/PageNotFound",
                "Employee/Index",
                "KPI/Index",
                "KPIGroup/Index",
                "Role/Index",
                "TargetTemplate/Index",
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
                "Discipline/Index",
                "RatingByKPI/Index",
                "Report/EPASummaryReport",
                "Report/EPADetailReport",
                "Report/MonthlyEPADetailReport",
                "Report/MonthlyEPASummaryReport",
                "EPAHistory/Index",
                "MonthlyPeriod/Index",
                "MonthlyPeriod/Update/{id}",
                "EPA/ResetScore/{id}",
                "ViewEPAByKPI/Index",
                "UpdateFinalScore/Index",
                "MonthlyEPAHistory/Index",
                "DisciplineSummary/Index"
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