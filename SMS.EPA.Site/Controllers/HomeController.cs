namespace SMS.EPA.Site.Controllers
{
    using System.Web.Mvc;
    using HRM.UserControl.Client.AspNetMvc.Session;
    using HRM.UserControl.Client.Core.Session;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;

    public class HomeController : Controller
    {
        private readonly IUserControlSession userControlSession;

        public HomeController()
            : this(null)
        {
        }

        public HomeController(IUserControlSession userControlSession)
        {
            this.userControlSession = userControlSession ?? new UserControlSession();
        }

        public ActionResult Index()
        {
            byte.TryParse(ConfigurationManagerService.GetAppSetting("SiteId"), out byte siteId);
            ViewBag.SiteId = siteId;
            var employee = userControlSession.Get<UserSession>(Constants.CurrentEmployeeKey);

            if (employee != null)
            {
                return View(employee);
            }

            return RedirectToAction("Login", "Auth");
        }
    }
}