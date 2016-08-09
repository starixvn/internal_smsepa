namespace SMS.EPA.Site.Controllers.API
{
    using System.Web.Http;
    using System.Web.Security;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Core.Services.Interfaces;

    [DeflateCompressionAttribute]
    public class BaseController : ApiController
    {
        protected ISessionService sessionService;

        public BaseController()
            : this(null)
        {
        }

        public BaseController(ISessionService sessionService)
        {
            this.sessionService = sessionService ?? new SessionService();
        }

        protected UserSession CurrentUser
        {
            get
            {
                var currentUser = sessionService.CurrentUser;
                if (currentUser == null)
                {
                    FormsAuthentication.RedirectToLoginPage();
                }

                return currentUser;
            }
        }
    }
}