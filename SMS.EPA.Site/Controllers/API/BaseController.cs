namespace SMS.EPA.Site.Controllers.API
{
    using System.Web.Http;
    using System.Web.Security;
    using SMS.EPA.Model.Authentication;
    using SMS.EPA.Web;
    using SMS.EPA.Web.ApiAuthorizeAttribute;

    [DeflateCompressionAttribute]
    public class BaseController : ApiController
    {
        protected UserSession CurrentUser
        {
            get
            {
                var currentUser = SessionManager.CurrentUser;
                if (currentUser == null)
                {
                    FormsAuthentication.RedirectToLoginPage();
                }

                return currentUser;
            }
        }
    }
}