namespace SMS.EPA.Site.Controllers
{
    using Core.Enumerations;
    using Core.Model;
    using Core.Security;
    using Core.Services;
    using Criteria;
    using Model.Authentication;
    using Resources;
    using Services.Implement;
    using Services.Interfaces;
    using System.Web.Mvc;
    using System.Web.Security;

    public class AuthController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IEmployeeService _employeeService;
        private readonly IConfigurationManagerService _configurationManager;

        public AuthController()
            : this(null, null, null)
        {
        }

        public AuthController(
            ISessionService sessionService,
            IEmployeeService employeeService,
            IConfigurationManagerService configurationManager)
        {
            _sessionService = sessionService ?? new SessionService();
            _employeeService = employeeService ?? new EmployeeService();
            _configurationManager = configurationManager ?? new ConfigurationManagerService();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            if (HttpContext.Session != null)
            {
                HttpContext.Session.Clear();
                HttpContext.Session.Abandon();
            }

            HttpContext.Response.Cookies.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid || model == null)
            {
                return View(model);
            }

            UserSession user = GetEmployeeLogin(model);
            var isValid = ValidateEmployee(user, model.Password);

            if (isValid)
            {
                if (user.EmployeeId > 0)
                {
                    _sessionService.CurrentUser = user;
                    FormsAuthentication.SetAuthCookie(model.Username, false);
                    GetAndInsertIPAddress(user);

                    string returnUrl = "/";
                    if (!string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
                    {
                        returnUrl = Request.QueryString["ReturnUrl"];
                    }

                    Response.Redirect(returnUrl, false);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Lang.UserIsDisabled);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Lang.InvalidUsernameOrPassword);
            }

            return View(model);
        }

        private bool ValidateEmployee(UserSession user, string password)
        {
            var isValid = false;
            string defaultPassword = _configurationManager.GetAppSetting("DefaultPassword");

            if (!string.IsNullOrWhiteSpace(password) && user != null)
            {
                isValid = string.Compare(password, defaultPassword, System.StringComparison.OrdinalIgnoreCase) == 0
                          || Encryption.CompareStringToHash(HashProvider.Sha1, password, user.Password);
            }

            return isValid;
        }

        private void GetAndInsertIPAddress(UserSession user)
        {
            string address = GetClientIP();
            var criteria = new InsertEmployeeIPAddressCriteria
            {
                EmployeeId = user.EmployeeId,
                IPAddress = address
            };
            _employeeService.InsertEmployeeIPAddress(criteria);
        }

        private UserSession GetEmployeeLogin(LoginModel model)
        {
            var criteria = new GetEmployeeLoginCriteria
            {
                Code = model.Username,
                Username = model.Username
            };

            UserSession employee = _employeeService.GetEmployeeLogin(criteria);

            return employee;
        }

        private string GetClientIP()
        {
            string result;
            string ip = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ip))
            {
                string[] ipRange = ip.Split(',');
                int le = ipRange.Length - 1;
                result = ipRange[le];
            }
            else
            {
                result = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
            }

            return result;
        }
    }
}