namespace SMS.EPA.ServicesAPI.Filters
{
    using Resources;
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using Utilities;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class IPHostValidationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
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
                }
            }
        }
    }
}