namespace SMS.EPA.Core.Attribute
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Web.Helpers;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class WebApiValidateAntiForgeryTokenAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            if (actionContext.Request.Method.Method != "GET")
            {
                var headers = actionContext.Request.Headers;
                var tokenCookie = headers
                    .GetCookies()
                    .Select(c => c[AntiForgeryConfig.CookieName])
                    .FirstOrDefault();

                var tokenHeader = string.Empty;
                if (headers.Contains("X-XSRF-Token"))
                {
                    tokenHeader = headers.GetValues("X-XSRF-Token").FirstOrDefault();
                }

                AntiForgery.Validate(tokenCookie?.Value, tokenHeader);
            }

            base.OnActionExecuting(actionContext);
        }
    }
}