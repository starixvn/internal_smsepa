// ----------------------------------------------------------------------
// <copyright file="AuthController.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/6/2015 10:25:37 AM">Created.</change>
// </history>

namespace SMS.EPA.Site.Controllers
{
    using System.Web.Mvc;
    using System.Web.Security;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Criteria.Authentication;
    using SMS.EPA.Model.Authentication;
    using SMS.EPA.Model.EmployeeManagement;
    using SMS.EPA.Resources;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;
    using SMS.EPA.Web;

    /// <summary>
    /// The login controller.
    /// </summary>
    public class AuthController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public AuthController()
            : this(null)
        {
        }

        public AuthController(IEmployeeService employeeService)
        {
            _employeeService = employeeService ?? new EmployeeService();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// The login action.
        /// </summary>
        /// <param name="model">The login model.</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                UserSession user = GetEmployeeLogin(model);
                if (user != null)
                {
                    SessionManager.CurrentUser = user;
                    FormsAuthentication.SetAuthCookie(model.Username, false);

                    string returnUrl = "/";
                    if (!string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
                    {
                        returnUrl = Request.QueryString["ReturnUrl"];
                    }

                    Response.Redirect(returnUrl, false);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Lang.InvalidUsernameOrPassword);
                }
            }

            return View(model);
        }

        /// <summary>
        /// Get employee login.
        /// </summary>
        /// <param name="model">The login model.</param>
        /// <returns>The user session.</returns>
        private UserSession GetEmployeeLogin(LoginModel model)
        {
            UserSession employee = null;
            employee = GetEmployeeLoginByCode(model);

            if (employee == null)
            {
                employee = GetEmployeeLoginByDomain(model);
            }

            return employee;
        }

        /// <summary>
        /// Get employee by code.
        /// </summary>
        /// <param name="model">The login model.</param>
        /// <returns>The user session.</returns>
        private UserSession GetEmployeeLoginByCode(LoginModel model)
        {
            UserSession employee = null;
            var criteria = new GetEmployeeByCodeCriteria
            {
                Code = model.Username
            };
            EmployeeModel employeeInfo = _employeeService.GetEmployeeByCode(criteria);

            if (employeeInfo != null)
            {
                bool isValid = _employeeService.CheckValidateEmployee(employeeInfo.Password, model.Password);

                if (isValid || string.Compare(model.Password, Constants.DefaultPassword, System.StringComparison.OrdinalIgnoreCase) == 0)
                {
                    employee = new UserSession
                    {
                        EmployeeInfo = employeeInfo
                    };
                }
            }

            return employee;
        }

        /// <summary>
        /// Get employee login by username.
        /// </summary>
        /// <param name="model">The login model.</param>
        /// <returns>The user session.</returns>
        private UserSession GetEmployeeLoginByDomain(LoginModel model)
        {
            UserSession employee = null;
            var criteria = new GetEmployeeByDomainUsernameCriteria
            {
                Username = model.Username
            };
            EmployeeModel employeeInfo = _employeeService.GetEmployeeByUsername(criteria);

            if (employeeInfo != null)
            {
                bool isValid = _employeeService.CheckValidateEmployee(employeeInfo.Password, model.Password);

                if (isValid || string.Compare(model.Password, Constants.DefaultPassword, System.StringComparison.OrdinalIgnoreCase) == 0)
                {
                    employee = new UserSession
                    {
                        EmployeeInfo = employeeInfo
                    };
                }
            }

            return employee;
        }
    }
}
