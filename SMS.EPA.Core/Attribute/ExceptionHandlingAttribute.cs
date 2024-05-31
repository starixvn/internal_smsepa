namespace SMS.EPA.Core.Attribute
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Filters;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;

    [AttributeUsage(AttributeTargets.All)]
    public sealed class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext != null)
            {
                var ex = actionExecutedContext.Exception;
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                    .ToString();

                if (ex is UnauthorizedAccessException)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                    {
                        Content = new StringContent(ex.Message),
                        ReasonPhrase = "UnauthorizedAccessException"
                    };
                    LogService.Info(logMessage);

                    throw new HttpResponseException(response);
                }
                else if (ex is OperationCanceledException)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent(ex.Message),
                        ReasonPhrase = "OperationCanceledException"
                    };
                    LogService.Info(logMessage);

                    throw new HttpResponseException(response);
                }
                else
                {
                    LogService.Error(logMessage);
                }
            }
        }
    }
}