namespace SMS.EPA.ServicesAPI.Filters
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using Resources;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class HttpsRequiredAttribute : ActionFilterAttribute
    {
        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            if (!string.Equals(actionContext.Request.RequestUri.Scheme, "https", StringComparison.OrdinalIgnoreCase))
            {
                using (var response = new HttpResponseMessage())
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Content = new StringContent(Lang.HTTPSRequired);

                    actionContext.Response = response;
                    return Task.FromResult(0);
                }
            }

            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }
    }
}