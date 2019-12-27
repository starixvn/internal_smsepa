namespace SMS.EPA.Core.Attribute
{
    using Helpers;
    using System;
    using System.Net.Http;
    using System.Web.Http.Filters;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class CustomActionFilterAttribute : ActionFilterAttribute
    {
        public override async void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext?.Response != null)
            {
                var content = actionExecutedContext.Response.Content;
                var bytes = await content?.ReadAsByteArrayAsync();
                var zlibbedContent = bytes == null
                    ? new byte[0]
                    : CompressionHelper.DeflateByte(bytes);
                actionExecutedContext.Response.Content = new ByteArrayContent(zlibbedContent);
                actionExecutedContext.Response.Content.Headers.Remove("Content-Type");
                actionExecutedContext.Response.Content.Headers.Add("Content-encoding", "deflate");
                actionExecutedContext.Response.Content.Headers.Add("Content-Type", "application/json");

                base.OnActionExecuted(actionExecutedContext);
            }
        }
    }
}