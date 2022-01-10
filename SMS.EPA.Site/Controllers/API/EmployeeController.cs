namespace SMS.EPA.Site.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models.User;
    using HRM.UserControl.Client.Core.Services;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Services;

    public class EmployeeController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly IUserService userService;

        public EmployeeController() : this(null, null, null, null)
        {
        }

        public EmployeeController(
            IEmployeeService employeeService,
            IBaseService baseService,
            ICryptoService cryptoService,
            IUserService userService)
        {
            this.employeeService = employeeService ?? new EmployeeService();
            this.baseService = baseService ?? new BaseService();
            this.cryptoService = cryptoService ?? new CryptoService();
            this.userService = userService ?? new UserService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Read)]
        public IEnumerable<Employee> GetAllEmployee(
            string query = "",
            string teamIds = null,
            bool? isLocal = null,
            bool? isSpecialUser = null,
            bool? isDisabledUser = null,
            bool? hasMonthlyEPA = null)
        {
            var criteria = new GetEmployeeCriteria
            {
                Query = query,
                TeamIds = teamIds,
                IsLocal = isLocal,
                IsSpecialUser = isSpecialUser,
                IsDisabledUser = isDisabledUser,
                HasMonthlyEPA = hasMonthlyEPA
            };
            return employeeService.GetEmployee(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Host)]
        public HttpResponseMessage CreateSpecialEmployee(CreateSpecialEmployeeCriteria criteria)
        {
            var password = criteria.Password;
            criteria.CreatedBy = CurrentEmployeeId;
            CommandResult commandResult = employeeService.CreateSpecialEmployee(criteria);
            if (commandResult.IsSuccessful)
            {
                var token = cryptoService.GenerateRijndaelKeyIV();
                string[] keyiv = token.Split(':');
                var rijndaelKey = keyiv[0];
                var rijndaelIV = keyiv[1];
                var result = userService.Create(new CreateUserParam
                {
                    Username = criteria.Username,
                    Nickname = criteria.NickName,
                    FullName = criteria.FullName,
                    Password = cryptoService.Encrypt(password, rijndaelKey, rijndaelIV),
                    Token = token
                });
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }

            return Request.CreateResponse(HttpStatusCode.OK, commandResult);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Host)]
        public HttpResponseMessage UpdateSpecialEmployee(UpdateSpecialEmployeeCriteria criteria)
        {
            criteria.ModifiedBy = CurrentEmployeeId;
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            var employee = employeeService.GetEmployeeById(criteria.EmployeeId);
            IList<User> users = userService.GetAll(employee.Username);
            User user = users.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Username)
                && x.Username.Equals(employee.Username, StringComparison.OrdinalIgnoreCase)
                && x.Nickname.Equals(employee.NickName, StringComparison.OrdinalIgnoreCase)
                && x.FullName.Equals(employee.FullName, StringComparison.OrdinalIgnoreCase));
            if(user != null)
            {
                var result = userService.UpdateInfo(new UpdateUserInfoParam
                {
                    UserId = user.UserId,
                    Code = employee.Code,
                    Nickname = criteria.NickName,
                    FullName = criteria.FullName,
                    Email = criteria.Email,
                    IsActive = criteria.Enabled
                });
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }

            return Request.CreateResponse(HttpStatusCode.OK, commandResult);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Host)]
        public CommandResult UpdateJobLevel(UpdateJobLevelCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }

        [HttpPut]
        public HttpResponseMessage ChangePassword(ChangePasswordCriteria criteria)
        {
            var password = criteria.NewPassword;
            criteria.EmployeeId = CurrentEmployeeId;
            criteria.ModifiedBy = CurrentEmployeeId;
            CommandResult commandResult = employeeService.ChangePassword(criteria);
            if (commandResult.IsSuccessful)
            {
                var token = cryptoService.GenerateRijndaelKeyIV();
                string[] keyiv = token.Split(':');
                var rijndaelKey = keyiv[0];
                var rijndaelIV = keyiv[1];
                var result = userService.UpdatePassword(new UpdatePasswordParam
                {
                    OldPassword = cryptoService.Encrypt(criteria.OldPassword, rijndaelKey, rijndaelIV),
                    Password = cryptoService.Encrypt(password, rijndaelKey, rijndaelIV),
                    Token = token
                });
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }

            return Request.CreateResponse(HttpStatusCode.OK, commandResult);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Host)]
        public HttpResponseMessage AssignRoleToEmployee(AssignRoleToEmployeeCriteria criteria)
        {
            criteria.ModifiedBy = CurrentEmployeeId;
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            if (commandResult.IsSuccessful)
            {
                var employee = employeeService.GetEmployeeById(criteria.EmployeeId);
                IList<User> users = userService.GetAll(employee.NickName);
                User user = employee.IsSpecialUser
                    ? users.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Username)
                        && x.Username.Equals(employee.Username, StringComparison.OrdinalIgnoreCase)
                        && x.Nickname.Equals(employee.NickName, StringComparison.OrdinalIgnoreCase)
                        && x.FullName.Equals(employee.FullName, StringComparison.OrdinalIgnoreCase))
                    : users.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Code)
                        && x.Code.Equals(employee.Code, StringComparison.OrdinalIgnoreCase)
                        && x.Nickname.Equals(employee.NickName, StringComparison.OrdinalIgnoreCase)
                        && x.FullName.Equals(employee.FullName, StringComparison.OrdinalIgnoreCase));
                var result = userService.AssignRole(new AssignRoleParam
                {
                    UserId = user != null ? user.UserId : 0,
                    RoleIds = criteria.RoleIds
                });
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }

            return Request.CreateResponse(HttpStatusCode.OK, commandResult);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Host)]
        public HttpResponseMessage ResetPassword(int employeeId)
        {
            string newPassword = ConfigurationManagerService.GetAppSetting("PasswordToReset");
            var criteria = new ResetPasswordCriteria
            {
                EmployeeId = employeeId,
                NewPassword = newPassword,
                ModifiedBy = CurrentEmployeeId
            };
            CommandResult commandResult = employeeService.ResetPassword(criteria);
            if (commandResult.IsSuccessful)
            {
                Employee employee = employeeService.GetEmployeeById(employeeId);
                IList<User> users = userService.GetAll(employee.Username);
                User user = users.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Username)
                    && x.Username.Equals(employee.Username, StringComparison.OrdinalIgnoreCase)
                    && x.Nickname.Equals(employee.NickName, StringComparison.OrdinalIgnoreCase)
                    && x.FullName.Equals(employee.FullName, StringComparison.OrdinalIgnoreCase));

                if(user != null)
                {
                    var token = cryptoService.GenerateRijndaelKeyIV();
                    string[] keyiv = token.Split(':');
                    var rijndaelKey = keyiv[0];
                    var rijndaelIV = keyiv[1];
                    var result = userService.ResetPassword(new UpdatePasswordParam
                    {
                        UserId = user.UserId,
                        Password = cryptoService.Encrypt(newPassword, rijndaelKey, rijndaelIV),
                        Token = token
                    });
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, commandResult);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Host)]
        public CommandResult UpdateHasMonthlyEPA(UpdateHasMonthlyEPACriteria criteria)
        {
            criteria.ModifiedBy = CurrentEmployeeId;
            return baseService.ExecuteNonQuery(criteria);
        }

        [HttpGet]
        public string GetDefaultPassword()
        {
            return ConfigurationManagerService.GetAppSetting("PasswordToReset");
        }

        public byte GetUserPermissionOnEmployee()
        {
            return GetPermissionOnFunction(EPAFunctionKey.UserManagement);
        }
    }
}