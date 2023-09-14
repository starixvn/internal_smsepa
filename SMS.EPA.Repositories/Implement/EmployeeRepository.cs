namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRM.UserControl.Client.Core.Models.Role;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;

    public class EmployeeRepository : IEmployeeRepository
    {
        public IEnumerable<Employee> GetAllEmployee(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria))
                {
                    var employees = multi.Read<Employee, JobTitle, JobLevel, Group, Employee>(
                           (emp, jb, jl, gr) =>
                           {
                               emp.JobTitle = jb;
                               emp.JobLevel = jl;
                               emp.Group = gr;

                               return emp;
                           },
                           "JobTitleId, JobLevelId, GroupId");

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
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria))
                {
                    var employee = multi.Read<Employee, JobTitle, Group, Employee>(
                           (emp, jb, gr) =>
                           {
                               emp.JobTitle = jb;
                               emp.Group = gr;
                               return emp;
                           },
                           "JobTitleId, GroupId").FirstOrDefault();

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