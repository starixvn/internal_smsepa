namespace SMS.EPA.Core.Attribute
{
    using Fanex.Logging;
    using SMS.EPA.Resources;
    using System;
    using System.Web;
    using System.Web.Http.Filters;

    [AttributeUsage(AttributeTargets.All)]
    public sealed class ExceptionHandlingAttribute : ExceptionFilterAttribute
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
                         Environment.NewLine + Lang.Source + exception.Source +
                        Environment.NewLine + Lang.Domain + HttpContext.Current.Request.Headers["Host"] +
                        Environment.NewLine + Lang.TargetSite + exception.TargetSite +
                        Environment.NewLine + Lang.StackTrace + exception.StackTrace;

                    LogService.Error(message);
                }
            }
        }
    }
}