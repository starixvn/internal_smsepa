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
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.RoleManagement;
    using SMS.EPA.Repositories.Interfaces;

    public class EmployeeRepository : IEmployeeRepository
    {
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

        public EmployeeModel GetEmployeeById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    EmployeeModel employee = multi.Read<EmployeeModel, JobTitle, DepartmentModel, Group, EmployeeModel>(
                           (emp, jb, dept, gr) =>
                           {
                               emp.JobTitle = jb;
                               emp.Department = dept;
                               emp.Group = gr;
                               return emp;
                           },
                           "JobTitleId, DepartmentId, GroupId").First();

                    if (employee != null)
                    {
                        employee.Roles = multi.Read<SystemRoleModel>().ToList();
                    }

                    return employee;
                }
            }
        }

        public IEnumerable<EmployeeModel> GetEmployeeByDepartmentId(ICriteria criteria)
        {
            List<EmployeeModel> employees = new List<EmployeeModel>();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                employees = db.Query<EmployeeModel>(criteria.GetSpParams()).ToList();
            }

            return employees;
        }

        public IEnumerable<EmployeeModel> GetEmployeeByGroupId(ICriteria criteria)
        {
            List<EmployeeModel> employees = new List<EmployeeModel>();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                employees = db.Query<EmployeeModel>(criteria.GetSpParams()).ToList();
            }

            return employees;
        }
    }
}