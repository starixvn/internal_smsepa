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

    public class EmployeeController : BaseController
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController()
            : this(null)
        {
        }

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService ?? new EmployeeService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.UserManagement, Policy = Policy.Read)]
        public IEnumerable<EmployeeModel> GetAllEmployee(string query = "", int? groupId = null, bool? isSpecialUser = null)
        {
            var criteria = new GetAllEmployeeCriteria
            {
                Query = query,
                GroupId = groupId,
                IsSpecialUser = isSpecialUser
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
        public CommandResult InsertSpecialEmployee(InsertSpecialEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = CurrentUser.EmployeeInfo.EmployeeId;
                criteria.Password = Encryption.ComputeHash(HashProvider.Sha1, criteria.Password);
            }

            CommandResult commandResult = _employeeService.InsertSpecialEmployee(criteria);
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
            var commandResult = new CommandResult();
            if (Encryption.CompareStringToHash(HashProvider.Sha1, criteria.OldPassword, CurrentUser.EmployeeInfo.Password))
            {
                if (criteria != null)
                {
                    criteria.EmployeeId = CurrentUser.EmployeeInfo.EmployeeId;
                    criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
                }

                commandResult = _employeeService.ChangePassword(criteria);
            }
            else
            {
                commandResult.Message = Lang.ValidateWrongPassword;
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
    }
}