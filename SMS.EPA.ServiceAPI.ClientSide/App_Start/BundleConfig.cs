namespace SMS.EPA.ServiceAPI.ClientSide
{
    using System.Web.Optimization;

    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.unobtrusive*",
                "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                "~/libraries/angular/angular.js",
                "~/libraries/angular-route/angular-route.js"));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/App/app.js",
                "~/App/routes.js",
                "~/App/token/tokenController.js",
                "~/App/department/departmentController.js",
                "~/App/token/tokenService.js",
                "~/App/department/departmentService.js"));


            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                 "~/Content/bootstrap.css",
                 "~/Content/Site.css"));
        }
    }
}
