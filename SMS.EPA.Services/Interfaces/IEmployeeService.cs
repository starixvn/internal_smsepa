// ----------------------------------------------------------------------
// <copyright file="IEmployeeService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/6/2015 2:20:53 PM">Created.</change>
// </history>

namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.Authentication;
    using SMS.EPA.Criteria.Employee;
    using SMS.EPA.Model.Authentication;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.Enitites;

    /// <summary>
    /// The interface of employee service.
    /// </summary>
    public interface IEmployeeService
    {
        UserSession GetEmployeeByCode(GetEmployeeByCodeCriteria criteria);

        UserSession GetEmployeeByUsername(GetEmployeeByDomainUsernameCriteria criteria);

        bool CheckValidateEmployee(string employeePassword, string enterPassword);

        IEnumerable<EmployeeModel> GetAllEmployee(GetAllEmployeeCriteria criteria);

        EmployeeModel GetEmployeeById(GetEmployeeByIdCriteria criteria);

        IEnumerable<Group> GetAllGroup(GetAllGroupCriteria criteria);

        CommandResult CreateSpecialEmployee(CreateSpecialEmployeeCriteria criteria);

        CommandResult UpdateSpecialEmployee(UpdateSpecialEmployeeCriteria criteria);

        IEnumerable<JobTitle> GetAllJobTitle(GetAllJobTitleCriteria criteria);

        CommandResult ChangePassword(ChangePasswordCriteria criteria);

        CommandResult AssignRoleToEmployee(AssignRoleToEmployeeCriteria criteria);

        IEnumerable<EmployeeModel> GetEmployeeByDepartmentId(GetEmployeeByDepartmentIdCriteria criteria);

        IEnumerable<EmployeeModel> GetEmployeeByGroupId(GetEmployeeByGroupIdCriteria criteria);

        IEnumerable<MenuGroupModel> GetMenu(GetMenuCriteria criteria);
    }
}