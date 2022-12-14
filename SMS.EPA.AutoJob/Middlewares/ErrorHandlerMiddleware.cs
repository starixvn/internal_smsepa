namespace SMS.EPA.AutoJob.Middlewares
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var message = string.Format(
                    "{0}\n\rSource: {1}\n\rDomain: {2}\n\rTargetSite: {3}\n\rStackTrace: {4}",
                    ex.Message,
                    ex.Source,
                    context.Request.Headers["Host"],
                    ex.TargetSite,
                    ex.StackTrace);
                logger.LogError(message);
            }
        }
    }
}