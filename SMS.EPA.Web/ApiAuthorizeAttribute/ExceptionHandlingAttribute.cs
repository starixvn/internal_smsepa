// ----------------------------------------------------------------------
// <copyright file="ExceptionHandlingAttribute.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/03/2015">Created.</change>
// </history>

namespace SMS.EPA.Web.ApiAuthorizeAttribute
{
    using System;
    using System.Web;
    using System.Web.Http.Filters;
    using Fanex.Logging;

    [AttributeUsage(AttributeTargets.All)]
    public class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var localFlag = actionExecutedContext.Request.Properties["MS_IsLocal"] as Lazy<bool>;
            if (localFlag == null || !localFlag.Value)
            {
                var exception = actionExecutedContext.Exception;
                string message = exception.Message +
                        "\r\nSource: " + exception.Source +
                        "\r\nDomain: " + HttpContext.Current.Request.Headers["Host"] +
                        "\r\nTargetSite: " + exception.TargetSite +
                        "\r\nStackTrace: " + exception.StackTrace;

                LogService.Error(message);
            }
        }
    }
}
