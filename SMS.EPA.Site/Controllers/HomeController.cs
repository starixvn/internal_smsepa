namespace SMS.EPA.Site.Controllers
{
    using System.Web.Mvc;
    using SMS.EPA.Model.Authentication;
    using SMS.EPA.Web;

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (SessionManager.CurrentUser != null)
            {
                UserSession user = SessionManager.CurrentUser;
                return View(user);
            }

            return RedirectToAction("Login", "Auth");
        }
    }
}