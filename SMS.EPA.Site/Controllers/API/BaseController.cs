namespace SMS.EPA.Site.Controllers.API
{
    using System.Web.Http;
    using System.Web.Security;
    using HRM.UserControl.Client.AspNetMvc.Session;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;

    [WebApiValidateAntiForgeryToken]
    [CustomActionFilter]
    public class BaseController : ApiController
    {
        private IUserControlSession userControlSession;

        public BaseController()
            : this(null)
        {
        }

        public BaseController(IUserControlSession userControlSession)
        {
            this.userControlSession = userControlSession ?? new UserControlSession();
        }

        public IUserControlSession UserControlSession
        {
            get
            {
                if (userControlSession == null || userControlSession.UserId <= 0)
                {
                    FormsAuthentication.RedirectToLoginPage();
                }

                return userControlSession;
            }

            set
            {
                userControlSession = value;
            }
        }

        protected int CurrentEmployeeId => UserControlSession.Get<int>(Constants.CurrentEmployeeIdKey);

        protected byte GetPermissionOnFunction(string functionKey)
        {
            byte policy = (byte)Policy.None;
            if (UserControlSession.HasPermissionOnFunction(functionKey, Policy.Host))
            {
                policy = (byte)Policy.Host;
            }
            else
            {
                if (UserControlSession.HasPermissionOnFunction(functionKey, Policy.Read))
                {
                    policy = (byte)Policy.Read;
                }
            }

            return policy;
        }
    }
}