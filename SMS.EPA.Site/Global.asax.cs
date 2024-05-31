namespace SMS.EPA.Site
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Net;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using System.Web.SessionState;
    using Fanex.Data;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Extensions.Web;
    using Fanex.Logging.Sentry.RavenClient;
    using HRM.UserControl.Client.AspNetMvc.Configurations;
    using HRM.UserControl.Client.AspNetMvc.Session;
    using HRM.UserControl.Client.Core;
    using SharpRaven;

    public class WebApiApplication : HttpApplication
    {
        private const string WebApiExecutionPath = "~/api";

        protected void Application_Start()
        {
            LogManager
                .SetDefaultLogCategory(ConfigurationManager.AppSettings["Sentry:Category"])
                .Use(new SentryLogging(new SentryEngineOptions
                {
                    Dsn = new Dsn(ConfigurationManager.AppSettings["Sentry:Dsn"]),
                    WriteLogTimeoutSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["Sentry:Timeout"])
                }));

            ExceptionHandler
                .UseConfig(new LoggingConfiguration())
                .GetHttpHeaderInfo()
                .GetHttpSessionInfo()
                .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);

            DbSettingProviderManager
                .StartNewSession()
                .WithSqlServer("~/App_Data/SpNames.xml")
                .Run();

            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

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
            var ex = Server.GetLastError();
            if (ex.GetType() == typeof(UnauthorizedAccessException))
            {
                var httpContext = HttpContext.Current;
                if (httpContext != null)
                {
                    RequestContext requestContext = ((MvcHandler)httpContext.CurrentHandler).RequestContext;
                    if (requestContext.HttpContext.Request.IsAjaxRequest())
                    {
                        httpContext.Response.Clear();
                        httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        httpContext.Response.End();
                    }
                    else
                    {
                        httpContext.Response.Redirect("~/Auth/Login");
                    }
                }
            }
            else
            {
                ExceptionHandler.Handle(ex, TraceEventType.Error);
            }
        }

        private static bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath != null
                && HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(WebApiExecutionPath, StringComparison.OrdinalIgnoreCase);
        }
    }
}