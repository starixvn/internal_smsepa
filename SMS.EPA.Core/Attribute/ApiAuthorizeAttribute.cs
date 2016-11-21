namespace SMS.EPA.Core.Attribute
{
    using System;
    using System.Collections.ObjectModel;
    using System.Net;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public sealed class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly ISessionService sessionService;

        public ApiAuthorizeAttribute()
            : this(null)
        {
        }

        public ApiAuthorizeAttribute(ISessionService sessionService)
        {
            this.sessionService = sessionService ?? new SessionService();
        }

        public string FunctionKey { get; set; }

        public Policy Policy { get; set; }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                UserSession currentUser = sessionService.CurrentUser;

                if (currentUser != null)
                {
                    CheckUserPermission(actionContext);
                    return;
                }
                else
                {
                    HandleUnauthorizedRequest(actionContext);
                }
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

        private void HandlePermissionRequest(HttpActionContext actionContext)
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);
            actionContext.Response = responseMessage;
        }

        private void CheckUserPermission(HttpActionContext actionContext)
        {
            Collection<ApiAuthorizeAttribute> attributes = actionContext.ActionDescriptor.GetCustomAttributes<ApiAuthorizeAttribute>();
            bool hasPermission = false;

            foreach (ApiAuthorizeAttribute item in attributes)
            {
                if (sessionService.HasPermissionOnFunction(item.FunctionKey, item.Policy))
                {
                    hasPermission = true;
                }
            }

            if (!hasPermission)
            {
                HandlePermissionRequest(actionContext);
            }
        }
    }
}