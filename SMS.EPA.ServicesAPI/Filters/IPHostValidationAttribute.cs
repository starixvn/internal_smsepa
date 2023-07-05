namespace SMS.EPA.ServicesAPI.Filters
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using Resources;
    using Utilities;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class IPHostValidationAttribute : ActionFilterAttribute
    {
        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            string userIP = actionContext.Request.GetOwinContext().Request.RemoteIpAddress;
            var supportedIps = AppConfig.SupportedIPs;
            var isExist = supportedIps.Any(i => i == userIP);
            if (supportedIps.Any() && !isExist)
            {
                using (var response = new HttpResponseMessage())
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Content = new StringContent(Lang.BadIPRequest);

                    actionContext.Response = response;
                    return Task.FromResult(0);
                }
            }

            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }
    }
}