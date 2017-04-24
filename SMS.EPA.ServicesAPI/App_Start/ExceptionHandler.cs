namespace SMS.EPA.ServicesAPI
{
    using Fanex.Logging;
    using Resources;
    using System;
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
                         Environment.NewLine + Lang.Source + exception.Source +
                        Environment.NewLine + Lang.Domain + HttpContext.Current.Request.Headers["Host"] +
                        Environment.NewLine + Lang.TargetSite + exception.TargetSite +
                        Environment.NewLine + Lang.StackTrace + exception.StackTrace;
            LogService.Error(message);

            using (var response = new HttpResponseMessage())
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Content = new StringContent(Lang.InternalServerError);

                context.Result = new ResponseMessageResult(response);
            }

            return Task.FromResult<object>(null);
        }
    }
}