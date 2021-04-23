namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using Core.Attribute;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Model.Enitites;
    using Services.Implement;
    using Services.Interfaces;

    public class EmployeeController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly IBaseService baseService;

        public EmployeeController() : this(null, null)
        {
        }

        public EmployeeController(
            IEmployeeService employeeService,
            IBaseService baseService)
        {
            this.employeeService = employeeService ?? new EmployeeService();
            this.baseService = baseService ?? new BaseService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Read)]
        public IEnumerable<Employee> GetAllEmployee(
            string query = "",
            int? departmentId = null,
            bool? isLocal = null,
            bool? isSpecialUser = null,
            bool? isDisabledUser = null,
            bool? hasMonthlyEPA = null)
        {
            var criteria = new GetAllEmployeeCriteria
            {
                Query = query,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                IsSpecialUser = isSpecialUser,
                IsDisabledUser = isDisabledUser,
                HasMonthlyEPA = hasMonthlyEPA
            };

            IEnumerable<Employee> employees = employeeService.GetAllEmployee(criteria);
            return employees;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Host)]
        public CommandResult CreateSpecialEmployee(CreateSpecialEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = employeeService.CreateSpecialEmployee(criteria);
            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Host)]
        public CommandResult UpdateSpecialEmployee(UpdateSpecialEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            return commandResult;
        }

        [HttpPut]
        public CommandResult ChangePassword(ChangePasswordCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.EmployeeId = SessionService.EmployeeId;
                criteria.ModifiedBy = SessionService.EmployeeId;
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
                criteria.ModifiedBy = SessionService.EmployeeId;
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
                ModifiedBy = SessionService.EmployeeId
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
                criteria.ModifiedBy = SessionService.EmployeeId;
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
            return SessionService.GetPermission(EPAFunctionKey.UserManagement);
        }
    }
}