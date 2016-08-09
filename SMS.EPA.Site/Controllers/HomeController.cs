namespace SMS.EPA.Site.Controllers
{
    using System.Web.Mvc;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Core.Services.Interfaces;

    public class HomeController : Controller
    {
        private ISessionService sessionService;

        public HomeController()
            : this(null)
        {
        }

        public HomeController(ISessionService sessionService)
        {
            this.sessionService = sessionService ?? new SessionService();
        }

        public ActionResult Index()
        {
            if (sessionService.CurrentUser != null)
            {
                UserSession user = sessionService.CurrentUser;
                return View(user);
            }

            return RedirectToAction("Login", "Auth");
        }
    }
}