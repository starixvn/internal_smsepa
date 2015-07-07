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
    using Fanex.Common.Helper;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Security;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.Authentication;
    using SMS.EPA.Model.EmployeeManagement;
    using SMS.EPA.Services.Interfaces;

    /// <summary>
    /// The employee service.
    /// </summary>
    public class EmployeeService : BaseService, IEmployeeService
    {
        /// <summary>
        /// Get employee by code.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The employee model.</returns>
        public EmployeeModel GetEmployeeByCode(GetEmployeeByCodeCriteria criteria)
        {
            var employee = new EmployeeModel();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                employee = GetDataEntity<EmployeeModel>(criteria);
            }

            return employee;
        }

        /// <summary>
        /// Get employee by username.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The employee model.</returns>
        public EmployeeModel GetEmployeeByUsername(GetEmployeeByDomainUsernameCriteria criteria)
        {
            var employee = new EmployeeModel();
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                employee = GetDataEntity<EmployeeModel>(criteria);
            }

            return employee;
        }

        public bool CheckValidateEmployee(string employeePassword, string enterPassword)
        {
            bool isValid = !string.IsNullOrWhiteSpace(enterPassword) &&
                                Encryption.CompareStringToHash(HashProvider.Sha1, enterPassword, employeePassword);
            return isValid;
        }
    }
}
