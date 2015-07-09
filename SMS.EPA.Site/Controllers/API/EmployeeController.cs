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
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Security;
    using SMS.EPA.Criteria.Employee;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    /// <summary>
    /// The employee controller.
    /// </summary>
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

        /// <summary>
        /// Get all employee.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="groupId">The group ID.</param>
        /// <param name="isSpecialUser">Is special user.</param>
        /// <returns>The enumerator employee model.</returns>
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

        /// <summary>
        /// Change password.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        [HttpPut]
        public CommandResult ChangePassword(ChangePasswordCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.EmployeeId = CurrentUser.EmployeeInfo.EmployeeId;
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            CommandResult commandResult = _employeeService.ChangePassword(criteria);
            return commandResult;
        }

        /// <summary>
        /// Assign role to employee.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        [HttpPut]
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
