namespace SMS.EPA.Site.Controllers
{
    using System;
    using System.Web.Mvc;
    using System.Web.Security;
    using Fanex.Data;
    using HRM.UserControl.Client.AspNetMvc.Configurations;
    using HRM.UserControl.Client.AspNetMvc.Session;
    using HRM.UserControl.Client.Core.Configurations;
    using HRM.UserControl.Client.Core.Models.Auth;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model.Authentication;

    public class AuthController : Controller
    {
        private readonly IUserControlSettings userControlSettings;
        private readonly IUserControlSession userControlSession;
        private readonly ICryptoService cryptoService;
        private readonly IAuthService authService;
        private readonly IUserService userService;
        private readonly IBaseService baseService;

        public AuthController()
            : this(null, null, null, null, null, null)
        {
        }

        public AuthController(
            IUserControlSettings userControlSettings,
            IUserControlSession userControlSession,
            ICryptoService cryptoService,
            IAuthService authService,
            IUserService userService,
            IBaseService baseService)
        {
            this.userControlSettings = userControlSettings ?? new UserControlSettings();
            this.userControlSession = userControlSession ?? new UserControlSession();
            this.cryptoService = cryptoService ?? new CryptoService();
            this.authService = authService ?? new AuthService();
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
                IP = GetClientIP(),
                Platform = Request.Browser.Platform,
                BrowserName = Request.Browser.Browser,
                NetSessionId = Session.SessionID
            };
            var loginResult = authService.Login(loginParam);
            if (loginResult.IsSuccessfull)
            {
                userControlSession.UserId = loginResult.UserId;
                userControlSession.SiteId = userControlSettings.SiteId;
                userControlSession.JwtToken = loginResult.JwtToken;
                userControlSession.MenuItems = userService.GetMenuItem();
                userControlSession.Functions = userService.GetFunctionPermission(userControlSession.SiteId, userControlSession.UserId);

                UserSession employee = GetEmployeeLogin();
                userControlSession.Set<UserSession>(Constants.CurrentEmployeeKey, employee);
                userControlSession.Set<int>(Constants.CurrentEmployeeIdKey, employee.EmployeeId);
                FormsAuthentication.SetAuthCookie(employee.EmployeeId.ToString(), false);
                Response.Redirect(Url.IsLocalUrl(model.ReturnPath) ? model.ReturnPath : FormsAuthentication.DefaultUrl);
            }
            else
            {
                ModelState.AddModelError(string.Empty, loginResult.ErrorMessage);
            }

            return View(model);
        }

        public ActionResult SSO(string userId, string onUserId, string time, string redirectUrl = null)
        {
            var userIdValue = Cryptography.DecryptPassword(userId.Replace(" ", "+"));
            var onUserIdValue = Cryptography.DecryptPassword(onUserId.Replace(" ", "+"));
            var timeValue = Cryptography.DecryptPassword(time.Replace(" ", "+"));
            var currentTime = authService.SSOGetTime();
            TimeSpan expiration = DateTime.ParseExact(currentTime, "yyyy-MM-dd HH:mm:ss.fff", null)
                - DateTime.ParseExact(timeValue, "yyyy-MM-dd HH:mm:ss.fff", null);
            const byte TimeExpiration = 30;
            if (expiration.TotalSeconds <= TimeExpiration)
            {
                var ssoParam = new SSOLoginParam
                {
                    UserId = Convert.ToInt32(userIdValue),
                    CheckOnUserId = Convert.ToInt64(onUserIdValue),
                    OnUserId = 0,
                    IP = GetClientIP(),
                    Platform = Request.Browser.Platform,
                    BrowserName = Request.Browser.Browser,
                    NetSessionId = Session.SessionID
                };
                LoginResult loginResult = authService.SSOLogin(ssoParam);
                if (loginResult.IsSuccessfull)
                {
                    userControlSession.UserId = Convert.ToInt32(userIdValue);
                    userControlSession.SiteId = userControlSettings.SiteId;
                    userControlSession.JwtToken = loginResult.JwtToken;
                    userControlSession.OnUserId = loginResult.OnUserId;
                    userControlSession.MenuItems = userService.GetMenuItem();
                    userControlSession.Functions = userService.GetFunctionPermission(userControlSession.SiteId, userControlSession.UserId);
                    UserSession employee = GetEmployeeLogin();

                    userControlSession.Set<UserSession>(Constants.CurrentEmployeeKey, employee);
                    userControlSession.Set<int>(Constants.CurrentEmployeeIdKey, employee.EmployeeId);
                    FormsAuthentication.SetAuthCookie(employee.EmployeeId.ToString(), false);
                    return !string.IsNullOrWhiteSpace(redirectUrl) && Url.IsLocalUrl(redirectUrl)
                        ? Redirect(string.Format("/#/{0}", redirectUrl)) : Redirect(FormsAuthentication.DefaultUrl);
                }
            }

            return RedirectToAction("Login");
        }

        private UserSession GetEmployeeLogin()
        {
            var criteria = new GetEmployeeLoginByUserIdCriteria
            {
                UserId = userControlSession.UserId
            };
            return baseService.GetDataEntity<UserSession>(criteria);
        }

        private string GetClientIP()
        {
            var clientIp = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (!string.IsNullOrWhiteSpace(clientIp))
            {
                var forwardedIps = clientIp.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                clientIp = forwardedIps[forwardedIps.Length - 1];
            }
            else
            {
                clientIp = Request.ServerVariables["REMOTE_ADDR"];
            }

            if (clientIp == "::1")
            {
                clientIp = "127.0.0.1";
            }

            return clientIp;
        }
    }
}