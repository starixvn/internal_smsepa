// ----------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/2/2015 10:41:22 AM">Created.</change>
// </history>

namespace SMS.EPA.Site
{
    using System;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using System.Web.SessionState;
    using Fanex.Data.Configuration;

    /// <summary>
    /// Web API Application.
    /// </summary>
    public class WebApiApplication : HttpApplication
    {
        private const string _WebApiPrefix = "api";
        private static string _webApiExecutionPath = string.Format("~/{0}", _WebApiPrefix);

        /// <summary>
        /// Application Start.
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DbSettingProviderManager.Current.Start();
        }

        protected void Application_PostAuthorizeRequest()
        {
            if (IsWebApiRequest())
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }

        private static bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(_webApiExecutionPath, StringComparison.OrdinalIgnoreCase);
        }
    }
}