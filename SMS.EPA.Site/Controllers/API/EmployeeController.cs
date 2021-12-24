namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
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
        public CommandResult UpdateSpecialEmployee(UpdateSpecialEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentEmployeeId;
            }

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Host)]
        public CommandResult UpdateJobLevel(UpdateJobLevelCriteria criteria)
        {

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            return commandResult;
        }

        [HttpPut]
        public CommandResult ChangePassword(ChangePasswordCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.EmployeeId = CurrentEmployeeId;
                criteria.ModifiedBy = CurrentEmployeeId;
            }

            CommandResult commandResult = employeeService.ChangePassword(criteria);

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Host)]
        public CommandResult AssignRoleToEmployee(AssignRoleToEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentEmployeeId;
            }

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            return commandResult;
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Host)]
        public CommandResult ResetPassword(int employeeId)
        {
            string newPassword = ConfigurationManagerService.GetAppSetting("PasswordToReset");
            var criteria = new ResetPasswordCriteria
            {
                EmployeeId = employeeId,
                NewPassword = newPassword,
                ModifiedBy = CurrentEmployeeId
            };

            CommandResult commandResult = employeeService.ResetPassword(criteria);
            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Host)]
        public CommandResult UpdateHasMonthlyEPA(UpdateHasMonthlyEPACriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentEmployeeId;
            }

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            return commandResult;
        }

        [HttpGet]
        public string GetDefaultPassword()
        {
            string defaultPassword = ConfigurationManagerService.GetAppSetting("PasswordToReset");
            return defaultPassword;
        }

        public byte GetUserPermissionOnEmployee()
        {
            return GetPermissionOnFunction(EPAFunctionKey.UserManagement);
        }
    }
}