namespace SMS.EPA.Site.Controllers.API
{
    using System.Web.Http;
    using System.Web.Security;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Core.Services.Interfaces;

    [DeflateCompressionAttribute]
    public class BaseController : ApiController
    {
        private ISessionService sessionService;

        public BaseController()
            : this(null)
        {
        }

        public BaseController(ISessionService sessionService)
        {
            this.sessionService = sessionService ?? new SessionService();
        }

        public ISessionService SessionService
        {
            get
            {
                if (sessionService == null || sessionService.EmployeeId <= 0)
                {
                    FormsAuthentication.RedirectToLoginPage();
                }

                return sessionService;
            }

            set
            {
                sessionService = value;
            }
        }
    }
}