// ----------------------------------------------------------------------
// <copyright file="IEmployeeRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/7/2015 10:51:50 AM">Created.</change>
// </history>

namespace SMS.EPA.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Authentication;
    using SMS.EPA.Model.Employee;

    /// <summary>
    /// The interface of employee repository.
    /// </summary>
    public interface IEmployeeRepository
    {
        RawEmployeeModel GetAllEmployee(ICriteria criteria);

        UserSession GetEmployee(ICriteria criteria);

        EmployeeModel GetEmployeeById(ICriteria criteria);
    }
}