namespace SMS.EPA.Core.Attribute
{
    using System;
    using System.Web;
    using System.Web.Http.Filters;
    using Fanex.Logging;

    [AttributeUsage(AttributeTargets.All)]
    public sealed class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext != null)
            {
                var ex = actionExecutedContext.Exception;
                var message = string.Format(
                    "{0}{1}Source: {2}{1}Domain: {3}{1}TargetSite: {4}{1}StackTrace: {5}",
                    ex.Message,
                    Environment.NewLine,
                    ex.Source,
                    HttpContext.Current.Request.Headers["Host"],
                    ex.TargetSite,
                    ex.StackTrace);

                LogService.Error(message);
            }
        }
    }
}