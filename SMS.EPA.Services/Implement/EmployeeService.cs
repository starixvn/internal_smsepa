// ----------------------------------------------------------------------
// <copyright file="EmployeeService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/6/2015 2:23:45 PM">Created.</change>
// </history>

namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Helper;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Security;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.Authentication;
    using SMS.EPA.Criteria.Employee;
    using SMS.EPA.Model.Authentication;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.RoleManagement;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;

    /// <summary>
    /// The employee service.
    /// </summary>
    public class EmployeeService : BaseService, IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService()
            : this(null)
        {
        }

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository ?? new EmployeeRepository();
        }

        /// <summary>
        /// Get employee by code.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The employee model contain employee information, roles and functions.</returns>
        public UserSession GetEmployeeByCode(GetEmployeeByCodeCriteria criteria)
        {
            var employee = new UserSession();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                employee = _employeeRepository.GetEmployee(criteria);
            }

            return employee;
        }

        /// <summary>
        /// Get employee by username.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The employee model contain employee information, roles and functions.</returns>
        public UserSession GetEmployeeByUsername(GetEmployeeByDomainUsernameCriteria criteria)
        {
            var employee = new UserSession();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                employee = _employeeRepository.GetEmployee(criteria);
            }

            return employee;
        }

        public bool CheckValidateEmployee(string employeePassword, string enterPassword)
        {
            bool isValid = !string.IsNullOrWhiteSpace(enterPassword) &&
                                Encryption.CompareStringToHash(HashProvider.Sha1, enterPassword, employeePassword);
            return isValid;
        }

        /// <summary>
        /// Get all employee.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The enumerator employee model.</returns>
        public IEnumerable<EmployeeModel> GetAllEmployee(GetAllEmployeeCriteria criteria)
        {
            IEnumerable<EmployeeModel> employees = new List<EmployeeModel>();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                RawEmployeeModel rawEmployee = _employeeRepository.GetAllEmployee(criteria);
                employees = GetRoleOfEmployee(rawEmployee);
            }

            return employees;
        }

        /// <summary>
        /// Get all group.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The enumerators group.</returns>
        public IEnumerable<Group> GetAllGroup(GetAllGroupCriteria criteria)
        {
            IEnumerable<Group> groups = new List<Group>();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                groups = GetData<Group>(criteria);
            }

            return groups;
        }

        /// <summary>
        /// Insert special employee.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        public CommandResult InsertSpecialEmployee(InsertSpecialEmployeeCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        /// <summary>
        /// Update special employee.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        public CommandResult UpdateSpecialEmployee(UpdateSpecialEmployeeCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        /// <summary>
        /// Get all job title.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The enumerator job title.</returns>
        public IEnumerable<JobTitle> GetAllJobTitle(GetAllJobTitleCriteria criteria)
        {
            IEnumerable<JobTitle> jobTitles = new List<JobTitle>();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                jobTitles = GetData<JobTitle>(criteria);
            }

            return jobTitles;
        }

        /// <summary>
        /// Change password.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        public CommandResult ChangePassword(ChangePasswordCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                criteria.NewPassword = Encryption.ComputeHash(HashProvider.Sha1, criteria.NewPassword);
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        /// <summary>
        /// Assign role to employee.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        public CommandResult AssignRoleToEmployee(AssignRoleToEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        /// <summary>
        /// Get roles of each employee.
        /// </summary>
        /// <param name="rawEmployee">The object contain employees and roles.</param>
        /// <returns>The list employees contain roles of each employee.</returns>
        private IEnumerable<EmployeeModel> GetRoleOfEmployee(RawEmployeeModel rawEmployee)
        {
            foreach (EmployeeModel employee in rawEmployee.Employees)
            {
                IEnumerable<RawEmployeeRoleModel> rolesOfCurrentEmployee = rawEmployee.Roles.Where(role => role.EmployeeId == employee.EmployeeId);

                foreach (RawEmployeeRoleModel rawRole in rolesOfCurrentEmployee)
                {
                    var role = new SystemRoleModel
                    {
                        RoleId = rawRole.RoleId,
                        RoleName = rawRole.RoleName
                    };

                    employee.Roles.Add(role);
                }
            }

            return rawEmployee.Employees;
        }
    }
}
