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
            if (actionExecutedContext != null)
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
}