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

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
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

        private static void HandlePermissionRequest(HttpActionContext actionContext)
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);
            actionContext.Response = responseMessage;
        }

        private void CheckUserPermission(HttpActionContext actionContext)
        {
            var hasPermission = actionContext.ActionDescriptor.GetCustomAttributes<ApiAuthorizeAttribute>().ToList()
                .Exists(x => userControlSession.HasPermissionOnFunction(x.FunctionKey, x.Policy));
            if (!hasPermission)
            {
                HandlePermissionRequest(actionContext);
            }
        }
    }
}