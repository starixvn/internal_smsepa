namespace SMS.EPA.Core.Attribute
{
    using System;
    using System.Web;
    using System.Web.Http.Filters;
    using Resources;

    [AttributeUsage(AttributeTargets.All)]
    public sealed class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ExceptionHandlingAttribute));

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

                    logger.Error(message);
                }
            }
        }
    }
}