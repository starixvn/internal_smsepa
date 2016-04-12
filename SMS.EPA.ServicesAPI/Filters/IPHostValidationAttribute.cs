namespace SMS.EPA.ServicesAPI.Filters
{
    using SMS.EPA.ServicesAPI.Utilities;
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    public class IPHostValidationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            string userIP = actionContext.Request.GetOwinContext().Request.RemoteIpAddress;
            try
            {
                var partnerIp = AppConfig.SupportedIP;
                var isPartner = partnerIp.Where(i => i == userIP).Any();
                if (!isPartner)
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Bad IP request")
                    };
                }
            }
            catch (Exception)
            {
                actionContext.Response =
                   new HttpResponseMessage(HttpStatusCode.Forbidden)
                   {
                       Content = new StringContent("Unauthorized IP Address")
                   };
                return;
            }
        }
    }
}