namespace SMS.EPA.Site.Controllers
{
    using System.Web.Mvc;
    using System.Web.Security;
    using HRM.UserControl.Client.AspNetMvc.Session;
    using HRM.UserControl.Client.Core.Models.Auth;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model.Authentication;
    using SMS.EPA.Services;

    public class AuthController : Controller
    {
        private readonly IUserControlSession userControlSession;
        private readonly ICryptoService cryptoService;
        private readonly ISecurityService securityService;
        private readonly IUserService userService;
        private readonly IBaseService baseService;

        public AuthController()
            : this(null, null, null, null, null)
        {
        }

        public AuthController(
            IUserControlSession userControlSession,
            ICryptoService cryptoService,
            ISecurityService securityService,
            IUserService userService,
            IBaseService baseService)
        {
            this.userControlSession = userControlSession ?? new UserControlSession();
            this.cryptoService = cryptoService ?? new CryptoService();
            this.securityService = securityService ?? new SecurityService();
            this.userService = userService ?? new UserService();
            this.baseService = baseService ?? new BaseService();
        }

        [HttpGet]
        public ActionResult Login()
        {
            byte.TryParse(ConfigurationManagerService.GetAppSetting("SiteId"), out byte siteId);
            ViewBag.SiteId = siteId;

            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            userControlSession.ClearAll();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            byte.TryParse(ConfigurationManagerService.GetAppSetting("SiteId"), out byte siteId);
            ViewBag.SiteId = siteId;

            if (!ModelState.IsValid || model == null)
            {
                return View(model);
            }

            var token = cryptoService.GenerateRijndaelKeyIV();
            string[] keyiv = token.Split(':');
            var rijndaelKey = keyiv[0];
            var rijndaelIV = keyiv[1];
            var loginParam = new LoginParam
            {
                Username = cryptoService.Encrypt(model.Username, rijndaelKey, rijndaelIV),
                Password = cryptoService.Encrypt(model.Password, rijndaelKey, rijndaelIV),
                Token = token,
                IP = Request.UserHostAddress,
                Platform = Request.Headers["User-Agent"],
                BrowserName = Request.Headers["User-Agent"]
            };
            var loginResult = securityService.Login(loginParam);
            if (loginResult.IsSuccessfull)
            {
                userControlSession.UserId = loginResult.UserId;
                userControlSession.SiteId = loginResult.SiteId;
                userControlSession.JwtToken = loginResult.JwtToken;
                userControlSession.MenuItems = userService.GetMenuItem();
                userControlSession.Functions = userService.GetFunctionPermission(userControlSession.SiteId, userControlSession.UserId);

                UserSession employee = GetEmployeeLogin();
                userControlSession.Set<UserSession>(Constants.CurrentSessionKey, employee);
                userControlSession.Set<int>(Constants.CurrentEmployeeIdKey, employee.EmployeeId);
                FormsAuthentication.SetAuthCookie(model.Username, false);
                Response.Redirect(Url.IsLocalUrl(model.ReturnPath) ? model.ReturnPath : FormsAuthentication.DefaultUrl);
            }
            else
            {
                ModelState.AddModelError(string.Empty, loginResult.ErrorMessage);
            }

            return View(model);
        }

        private UserSession GetEmployeeLogin()
        {
            var criteria = new GetEmployeeLoginByUserIdCriteria
            {
                UserId = userControlSession.UserId
            };
            return baseService.GetDataEntity<UserSession>(criteria);
        }
    }
}