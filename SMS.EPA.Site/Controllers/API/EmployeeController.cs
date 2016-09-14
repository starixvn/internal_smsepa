namespace SMS.EPA.Site.Controllers.API
{
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;
    using System.Collections.Generic;
    using System.Web.Http;

    public class EmployeeController : BaseController
    {
        private readonly IEmployeeService employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService ?? new EmployeeService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Read)]
        public IEnumerable<Employee> GetAllEmployee(
            string query = "",
            int? groupId = null,
            bool? isLocal = null,
            bool? isSpecialUser = null,
            bool? isDisabledUser = null,
            bool? hasMonthlyEPA = null)
        {
            var criteria = new GetAllEmployeeCriteria
            {
                Query = query,
                GroupId = groupId,
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
                criteria.CreatedBy = this.SessionService.EmployeeId;
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
                criteria.ModifiedBy = this.SessionService.EmployeeId;
            }

            CommandResult commandResult = employeeService.UpdateSpecialEmployee(criteria);
            return commandResult;
        }

        [HttpPut]
        public CommandResult ChangePassword(ChangePasswordCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.EmployeeId = this.SessionService.EmployeeId;
                criteria.ModifiedBy = this.SessionService.EmployeeId;
            }

            CommandResult commandResult = employeeService.ChangePassword(criteria);

            /*if (commandResult.IsSuccessful)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, commandResult);
            }*/

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Host)]
        public CommandResult AssignRoleToEmployee(AssignRoleToEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = this.SessionService.EmployeeId;
            }

            CommandResult commandResult = employeeService.AssignRoleToEmployee(criteria);
            return commandResult;
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Host)]
        public CommandResult ResetPassword(int employeeId)
        {
            string oldPassword = employeeService.GetPasswordByEmployeeId(employeeId);
            var criteria = new ChangePasswordCriteria
            {
                EmployeeId = employeeId,
                OldPassword = oldPassword,
                NewPassword = Constants.ResetDefaultPassword,
                ConfirmPassword = Constants.ResetDefaultPassword,
                ModifiedBy = this.SessionService.EmployeeId
            };

            CommandResult commandResult = employeeService.ChangePassword(criteria);
            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Host)]
        public CommandResult UpdateHasMonthlyEPA(UpdateHasMonthlyEPACriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = this.SessionService.EmployeeId;
            }

            CommandResult commandResult = employeeService.UpdateHasMonthlyEPA(criteria);
            return commandResult;
        }

        [HttpGet]
        public string GetDefaultPassword()
        {
            string defaultPassword = Constants.ResetDefaultPassword;
            return defaultPassword;
        }

        public int GetUserPermissionOnEmployee()
        {
            int permission = this.SessionService.GetPermission(EPAFunctionKey.UserManagement);
            return permission;
        }
    }
}