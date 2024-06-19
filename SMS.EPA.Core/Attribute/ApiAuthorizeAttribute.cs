namespace SMS.EPA.Core.Attribute
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using HRM.UserControl.Client.AspNetMvc.Session;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly IUserControlSession userControlSession;

        public ApiAuthorizeAttribute()
            : this(null)
        {
        }

        public ApiAuthorizeAttribute(IUserControlSession userControlSession)
        {
            this.userControlSession = userControlSession ?? new UserControlSession();
        }

        public string FunctionKey { get; set; }

        public Policy Policy { get; set; }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (userControlSession.UserId > 0)
                {
                    CheckUserPermission(actionContext);
                    return;
                }

                HandleUnauthorizedRequest(actionContext);
            }
            else
            {
                HandleUnauthorizedRequest(actionContext);
            }
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            if (actionContext != null)
            {
                var responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                actionContext.Response = responseMessage;
            }
        }

        private void CheckUserPermission(HttpActionContext actionContext)
        {
            bool hasPermission = false;
            if (!string.IsNullOrWhiteSpace(FunctionKey))
            {
                var functionKeys = FunctionKey.Split(',').ToList();
                hasPermission = functionKeys.Exists(x => userControlSession.HasPermissionOnFunction(x, Policy));
            }

            if (!hasPermission)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            }
        }
    }
}