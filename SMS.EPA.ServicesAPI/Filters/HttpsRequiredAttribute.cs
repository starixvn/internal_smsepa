namespace SMS.EPA.ServicesAPI.Filters
{
    using SMS.EPA.Resources;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class HttpsRequiredAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!string.Equals(actionContext.Request.RequestUri.Scheme, "https", StringComparison.OrdinalIgnoreCase))
            {
                using (var response = new HttpResponseMessage())
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Content = new StringContent(Lang.HTTPSRequired);

                    actionContext.Response = response;
                }
            }
        }
    }
}