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
    using SMS.EPA.Model.EmployeeManagement;
    using SMS.EPA.Model.Enitites;

    /// <summary>
    /// The interface of employee service.
    /// </summary>
    public interface IEmployeeService
    {
        EmployeeModel GetEmployeeByCode(GetEmployeeByCodeCriteria criteria);

        EmployeeModel GetEmployeeByUsername(GetEmployeeByDomainUsernameCriteria criteria);

        bool CheckValidateEmployee(string employeePassword, string enterPassword);

        IEnumerable<EmployeeModel> GetAllEmployee(GetAllEmployeeCriteria criteria);

        IEnumerable<Group> GetAllGroup(GetAllGroupCriteria criteria);

        CommandResult InsertSpecialEmployee(InsertSpecialEmployeeCriteria criteria);

        CommandResult UpdateSpecialEmployee(UpdateSpecialEmployeeCriteria criteria);
    }
}
