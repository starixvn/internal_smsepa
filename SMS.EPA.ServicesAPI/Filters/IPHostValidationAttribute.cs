namespace SMS.EPA.ServicesAPI.Filters
{
    using SMS.EPA.Resources;
    using SMS.EPA.ServicesAPI.Utilities;
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class IPHostValidationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            string userIP = actionContext.Request.GetOwinContext().Request.RemoteIpAddress;
            var supportedIps = AppConfig.SupportedIPs;
            var isExist = supportedIps.Where(i => i == userIP).Any();
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