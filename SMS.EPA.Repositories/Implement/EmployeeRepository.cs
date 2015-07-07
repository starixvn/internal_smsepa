// ----------------------------------------------------------------------
// <copyright file="EmployeeRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/7/2015 10:53:33 AM">Created.</change>
// </history>

namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.EmployeeManagement;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Repositories.Interfaces;

    /// <summary>
    /// The employee repository.
    /// </summary>
    public class EmployeeRepository : IEmployeeRepository
    {
        /// <summary>
        /// Get all employee.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The enumerator employee model.</returns>
        public IEnumerable<EmployeeModel> GetAllEmployee(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<EmployeeModel, JobTitle, Group, EmployeeModel>(
                    (emp, jb, gr) =>
                    {
                        emp.JobTitle = jb;
                        emp.Group = gr;
                        return emp;
                    },
                    "JobTitleId, GroupId",
                    criteria.GetSpParams());
            }
        }
    }
}
