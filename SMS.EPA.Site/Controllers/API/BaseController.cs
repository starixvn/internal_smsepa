namespace SMS.EPA.Site.Controllers.API
{
    using Core.Attribute;
    using Core.Services;
    using System.Web.Http;
    using System.Web.Security;

    [DeflateCompression]
    public class BaseController : ApiController
    {
        private ISessionService _sessionService;

        public BaseController()
            : this(null)
        {
        }

        public BaseController(ISessionService sessionService)
        {
            _sessionService = sessionService ?? new SessionService();
        }

        public ISessionService SessionService
        {
            get
            {
                if (_sessionService == null || _sessionService.EmployeeId <= 0)
                {
                    FormsAuthentication.RedirectToLoginPage();
                }

                return _sessionService;
            }

            set
            {
                _sessionService = value;
            }
        }
    }
}