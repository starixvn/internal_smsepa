namespace SMS.EPA.ServicesAPI.Filters
{
    using Fanex.Logging;
    using System.Net;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http.Filters;

    public class ExceptionHandler : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var exception = actionExecutedContext.Exception;

            string message = exception.Message +
                    "\r\nSource: " + exception.Source +
                    "\r\nDomain: " + HttpContext.Current.Request.Headers["Host"] +
                    "\r\nTargetSite: " + exception.TargetSite +
                    "\r\nStackTrace: " + exception.StackTrace;

            LogService.Error(message);

            actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("Sorry for the inconvenience! Please contact Admin to supported!")
            };
        }
    }
}