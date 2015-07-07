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
    using SMS.EPA.Criteria.Authentication;
    using SMS.EPA.Model.EmployeeManagement;

    /// <summary>
    /// The interface of employee service.
    /// </summary>
    public interface IEmployeeService
    {
        EmployeeModel GetEmployeeByCode(GetEmployeeByCodeCriteria criteria);

        EmployeeModel GetEmployeeByUsername(GetEmployeeByDomainUsernameCriteria criteria);

        bool CheckValidateEmployee(string employeePassword, string enterPassword);
    }
}
