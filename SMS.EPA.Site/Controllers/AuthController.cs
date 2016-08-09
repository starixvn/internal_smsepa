namespace SMS.EPA.Site.Controllers
{
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Core.Services.Interfaces;
    using SMS.EPA.Criteria;
    using SMS.EPA.Criteria.Authentication;
    using SMS.EPA.Model.Authentication;
    using SMS.EPA.Resources;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class AuthController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IEmployeeService employeeService;

        public AuthController()
            : this(null, null)
        {
        }

        public AuthController(ISessionService sessionService, IEmployeeService employeeService)
        {
            this.sessionService = sessionService ?? new SessionService();
            this.employeeService = employeeService ?? new EmployeeService();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Abandon();
            HttpContext.Response.Cookies.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid && model != null)
            {
                UserSession user = GetEmployeeLogin(model);
                if (user != null)
                {
                    if (user.EmployeeId > 0)
                    {
                        this.sessionService.CurrentUser = user;
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
            }

            return View(model);
        }

        private void GetAndInsertIPAddress(UserSession user)
        {
            string address = GetClientIP();
            var criteria = new InsertEmployeeIPAddressCriteria
            {
                EmployeeId = user.EmployeeId,
                IPAddress = address
            };
            employeeService.InsertEmployeeIPAddress(criteria);
        }

        private UserSession GetEmployeeLogin(LoginModel model)
        {
            var criteria = new GetEmployeeLoginCriteria
            {
                Code = model.Username,
                Username = model.Username
            };

            UserSession employee = employeeService.GetEmployeeLogin(criteria);

            if (employee != null && string.Compare(model.Password, Constants.DefaultPassword, System.StringComparison.OrdinalIgnoreCase) != 0)
            {
                bool isValid = employeeService.CheckValidateEmployee(employee.Password, model.Password);

                if (!isValid)
                {
                    employee = null;
                }
            }

            return employee;
        }

        private string GetClientIP()
        {
            string result = string.Empty;
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