namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRM.UserControl.Client.Core.Models.Role;
    using Model;
    using Model.Enitites;

    public class EmployeeRepository : IEmployeeRepository
    {
        public IEnumerable<Employee> GetAllEmployee(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var employees = multi.Read<Employee, JobTitle, JobLevel, Group, Department, Employee>(
                           (emp, jb, jl, gr, dept) =>
                           {
                               emp.JobTitle = jb;
                               emp.JobLevel = jl;
                               emp.Group = gr;
                               emp.Department = dept;

                               return emp;
                           },
                           "JobTitleId, JobLevelId, GroupId, DepartmentId");

                    var roles = multi.Read<EmployeeRoleModel>();

                    foreach (Employee employee in employees)
                    {
                        IList<Role> employeeRoles = roles.Where(role => role.EmployeeId == employee.EmployeeId)
                            .Select(role => new Role
                            {
                                RoleId = role.RoleId,
                                Name = role.RoleName
                            }).ToList();

                        employee.Roles = employeeRoles;
                    }

                    return employees;
                }
            }
        }

        public Employee GetEmployeeById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var employee = multi.Read<Employee, JobTitle, Department, Group, Employee>(
                           (emp, jb, dept, gr) =>
                           {
                               emp.JobTitle = jb;
                               emp.Department = dept;
                               emp.Group = gr;
                               return emp;
                           },
                           "JobTitleId, DepartmentId, GroupId").FirstOrDefault();

                    if (employee != null)
                    {
                        employee.Roles = multi.Read<Role>().ToList();
                    }

                    return employee;
                }
            }
        }
    }
}