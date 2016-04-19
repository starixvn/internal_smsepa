namespace SMS.EPA.ServicesAPI
{
    using Fanex.Logging;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http.ExceptionHandling;
    using System.Web.Http.Results;

    public class GlobalExceptionHandler : IExceptionHandler
    {
        public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            var exception = context.Exception;

            string message = exception.Message +
                    "\r\nSource: " + exception.Source +
                    "\r\nDomain: " + HttpContext.Current.Request.Headers["Host"] +
                    "\r\nTargetSite: " + exception.TargetSite +
                    "\r\nStackTrace: " + exception.StackTrace;
            LogService.Error(message);

            var response = context.Request.CreateResponse(
                HttpStatusCode.InternalServerError,
                "Sorry for the inconvenience! Please contact Admin to supported!");

            context.Result = new ResponseMessageResult(response);

            return Task.FromResult<object>(null);
        }
    }
}