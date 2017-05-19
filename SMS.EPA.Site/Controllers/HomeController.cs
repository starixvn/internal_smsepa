namespace SMS.EPA.Site.Controllers
{
    using Core.Model;
    using Core.Services;
    using System.Web.Mvc;

    public class HomeController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IConfigurationManagerService _configurationManager;

        public HomeController()
            : this(null, null)
        {
        }

        public HomeController(ISessionService sessionService, IConfigurationManagerService configurationManager)
        {
            _sessionService = sessionService ?? new SessionService();
            _configurationManager = configurationManager ?? new ConfigurationManagerService();
        }

        public ActionResult Index()
        {
            int siteId = 0;
            int.TryParse(_configurationManager.GetAppSetting("SiteId"), out siteId);
            ViewBag.SiteId = siteId;

            if (_sessionService.CurrentUser != null)
            {
                UserSession user = _sessionService.CurrentUser;
                return View(user);
            }

            return RedirectToAction("Login", "Auth");
        }
    }
}