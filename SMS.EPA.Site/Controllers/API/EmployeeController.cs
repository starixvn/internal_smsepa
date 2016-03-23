// ----------------------------------------------------------------------
// <copyright file="EmployeeController.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/7/2015 11:00:47 AM">Created.</change>
// </history>

namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Security;
    using SMS.EPA.Criteria.Employee;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Resources;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;
    using SMS.EPA.Web.ApiAuthorizeAttribute;
    using SMS.EPA.Web.Services;

    public class EmployeeController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IWebCommonService _webCommonService;

        public EmployeeController()
            : this(null, null)
        {
        }

        public EmployeeController(
            IEmployeeService employeeService,
            IWebCommonService webCommonService)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _webCommonService = webCommonService ?? new WebCommonService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Read)]
        public IEnumerable<EmployeeModel> GetAllEmployee(
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

            IEnumerable<EmployeeModel> employees = _employeeService.GetAllEmployee(criteria);

            return employees;
        }

        public IEnumerable<Group> GetAllGroup()
        {
            var criteria = new GetAllGroupCriteria();
            IEnumerable<Group> groups = _employeeService.GetAllGroup(criteria);

            return groups;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Host)]
        public CommandResult CreateSpecialEmployee(CreateSpecialEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = CurrentUser.EmployeeInfo.EmployeeId;
                criteria.Password = Encryption.ComputeHash(HashProvider.Sha1, criteria.Password);
            }

            CommandResult commandResult = _employeeService.CreateSpecialEmployee(criteria);
            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Host)]
        public CommandResult UpdateSpecialEmployee(UpdateSpecialEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            CommandResult commandResult = _employeeService.UpdateSpecialEmployee(criteria);
            return commandResult;
        }

        public IEnumerable<JobTitle> GetAllJobTitle()
        {
            var criteria = new GetAllJobTitleCriteria();
            IEnumerable<JobTitle> jobTitles = _employeeService.GetAllJobTitle(criteria);
            return jobTitles;
        }

        [HttpPut]
        public CommandResult ChangePassword(ChangePasswordCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();
            if (criteria != null)
            {
                if (Encryption.CompareStringToHash(HashProvider.Sha1, criteria.OldPassword, CurrentUser.EmployeeInfo.Password))
                {
                    criteria.EmployeeId = CurrentUser.EmployeeInfo.EmployeeId;
                    criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;

                    commandResult = _employeeService.ChangePassword(criteria);
                }
                else
                {
                    commandResult.Message = Lang.ValidateWrongPassword;
                }
            }

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Host)]
        public CommandResult AssignRoleToEmployee(AssignRoleToEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            CommandResult commandResult = _employeeService.AssignRoleToEmployee(criteria);

            return commandResult;
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Host)]
        public CommandResult ResetPassword(int employeeId)
        {
            var criteria = new ChangePasswordCriteria()
            {
                EmployeeId = employeeId,
                OldPassword = CurrentUser.EmployeeInfo.Password,
                NewPassword = Constants.ResetDefaultPassword,
                ConfirmPassword = Constants.ResetDefaultPassword,
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
            };

            return _employeeService.ChangePassword(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Host)]
        public CommandResult UpdateHasMonthlyEPA(UpdateHasMonthlyEPACriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            CommandResult commandResult = _employeeService.UpdateHasMonthlyEPA(criteria);

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
            int permission = _webCommonService.GetPermission(EPAFunctionKey.UserManagement);
            return permission;
        }
    }
}