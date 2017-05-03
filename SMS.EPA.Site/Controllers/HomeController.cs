namespace SMS.EPA.Site.Controllers
{
    using Core.Model;
    using Core.Services;
    using System.Web.Mvc;

    public class HomeController : Controller
    {
        private readonly ISessionService _sessionService;

        public HomeController()
            : this(null)
        {
        }

        public HomeController(ISessionService sessionService)
        {
            _sessionService = sessionService ?? new SessionService();
        }

        public ActionResult Index()
        {
            if (_sessionService.CurrentUser != null)
            {
                UserSession user = _sessionService.CurrentUser;
                return View(user);
            }

            return RedirectToAction("Login", "Auth");
        }
    }
}