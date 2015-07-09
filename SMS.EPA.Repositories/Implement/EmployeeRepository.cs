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
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Authentication;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.RoleManagement;
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
        /// <returns>The raw employee model.</returns>
        public RawEmployeeModel GetAllEmployee(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<EmployeeModel> employees = multi.Read<EmployeeModel, JobTitle, Group, EmployeeModel>(
                           (emp, jb, gr) =>
                           {
                               emp.JobTitle = jb;
                               emp.Group = gr;
                               return emp;
                           },
                           "JobTitleId, GroupId");
                    IEnumerable<RawEmployeeRoleModel> roles = multi.Read<RawEmployeeRoleModel>();

                    var rawEmployee = new RawEmployeeModel
                    {
                        Employees = employees,
                        Roles = roles
                    };

                    return rawEmployee;
                }
            }
        }

        /// <summary>
        /// Get employee by code or by domain username.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The employee.</returns>
        public UserSession GetEmployee(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    UserSession userSession = null;
                    IEnumerable<EmployeeModel> employees = multi.Read<EmployeeModel, JobTitle, Group, EmployeeModel>(
                           (emp, jb, gr) =>
                           {
                               emp.JobTitle = jb;
                               emp.Group = gr;
                               return emp;
                           },
                           "JobTitleId, GroupId");

                    if (employees != null && employees.Count() > 0)
                    {
                        EmployeeModel employee = employees.First();
                        IEnumerable<SystemRoleModel> roles = multi.Read<SystemRoleModel>();
                        IEnumerable<FunctionModel> functions = multi.Read<FunctionModel>();

                        userSession = new UserSession
                        {
                            EmployeeInfo = employee,
                            Roles = roles,
                            Functions = functions
                        };
                    }

                    return userSession;
                }
            }
        }
    }
}
