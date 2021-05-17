namespace SMS.EPA.ServicesAPI
{
    using System.Globalization;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http.ExceptionHandling;
    using System.Web.Http.Results;
    using Resources;

    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(GlobalExceptionHandler));

        public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            var exception = context.Exception;

            string message = string.Format(
                CultureInfo.InvariantCulture,
                "{0}\r\nSource: {1}\r\nDomain: {2}\r\nTargetSite: {3}\r\nStackTrace: {4}",
                exception.Message,
                exception.Source,
                HttpContext.Current.Request.Headers["Host"],
                exception.TargetSite,
                exception.StackTrace);

            logger.Error(message);

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