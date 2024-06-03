namespace SMS.EPA.Core.Attribute
{
    using System;
    using System.Linq;
    using System.Web.Http.Filters;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class CompressContentAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var acceptedEncoding = actionExecutedContext.Response.RequestMessage.Headers.AcceptEncoding.First().Value;
            if (!acceptedEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase)
                && !acceptedEncoding.Equals("deflate", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            actionExecutedContext.Response.Content = new CompressedContent(actionExecutedContext.Response.Content, acceptedEncoding);
        }
    }
}