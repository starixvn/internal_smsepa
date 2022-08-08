namespace SMS.EPA.Site
{
    using System;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using System.Web.SessionState;
    using Fanex.Data;
    using Fanex.Data.SqlServer;
    using HRM.UserControl.Client.AspNetMvc.Configurations;
    using HRM.UserControl.Client.AspNetMvc.Session;
    using HRM.UserControl.Client.Core;
    using Resources;

    public class WebApiApplication : HttpApplication
    {
        private const string WebApiExecutionPath = "~/api";
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(WebApiApplication));

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DbSettingProviderManager
                .StartNewSession()
                .WithSqlServer("~/App_Data/SpNames.xml")
                .Run();

            log4net.Config.XmlConfigurator.Configure();

            UserControlClientManager
               .UseSettings(new UserControlSettings())
               .UseSession(new UserControlSession());
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
            var message = string.Format(
                "{0}{1}{2}{3}{1}{4}{5}{1}{6}{7}{1}{8}{9}",
                ex.Message,
                Environment.NewLine,
                Lang.Source,
                ex.Source,
                Lang.Domain,
                HttpContext.Current.Request.Headers["Host"],
                Lang.TargetSite,
                ex.TargetSite,
                Lang.StackTrace,
                ex.StackTrace);

            logger.Error(message);
        }

        private static bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath != null
                && HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(WebApiExecutionPath, StringComparison.OrdinalIgnoreCase);
        }
    }
}