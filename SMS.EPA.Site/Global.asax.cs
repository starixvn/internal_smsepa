namespace SMS.EPA.Site
{
    using Fanex.Data;
    using Fanex.Data.OldConnectionStringProvider;
    using Fanex.Logging;
    using Resources;
    using System;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using System.Web.SessionState;

    public class WebApiApplication : HttpApplication
    {
        private const string WebApiPrefix = "api";
        private static readonly string WebApiExecutionPath = $"~/{WebApiPrefix}";

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DbSettingProviderManager
            .StartNewSession()
            .Use(new OldConnectionStringProvider())
            .UseDefaultDbSettingProvider(
                resourcePath: "~/App_Data",
                ignoreRedundantParameters: false,
                enableWatching: true)
            .Run();
        }

        protected void Application_PostAuthorizeRequest()
        {
            if (IsWebApiRequest())
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError().GetBaseException();
            string message = ex.Message +
                         Environment.NewLine + Lang.Source + ex.Source +
                        Environment.NewLine + Lang.Domain + HttpContext.Current.Request.Headers["Host"] +
                        Environment.NewLine + Lang.TargetSite + ex.TargetSite +
                        Environment.NewLine + Lang.StackTrace + ex.StackTrace;

            LogService.Error(message);
        }

        private static bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath != null
                && HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(WebApiExecutionPath, StringComparison.OrdinalIgnoreCase);
        }
    }
}