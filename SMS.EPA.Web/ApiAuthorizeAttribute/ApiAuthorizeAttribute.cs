namespace SMS.EPA.Web.ApiAuthorizeAttribute
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model.Authentication;
    using SMS.EPA.Web.Services;

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    [SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        private string _functionKey;
        private IWebCommonService _webCommonService;

        public ApiAuthorizeAttribute()
            : this(null)
        {
        }

        public ApiAuthorizeAttribute(IWebCommonService webCommonService)
        {
            _webCommonService = webCommonService ?? new WebCommonService();
        }

        public string FunctionKey
        {
            get
            {
                return _functionKey != null ? _functionKey : string.Empty;
            }

            set
            {
                _functionKey = value;
            }
        }

        public Policy Policy { get; set; }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                UserSession currentUser = SessionManager.CurrentUser;

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
                if (_webCommonService.HasPermissionOnFunction(item.FunctionKey, item.Policy))
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