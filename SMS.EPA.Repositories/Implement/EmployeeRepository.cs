namespace SMS.EPA.Repositories
{
    using Core.Model;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;
    using Model.Enitites;
    using System.Collections.Generic;
    using System.Linq;

    public class EmployeeRepository : IEmployeeRepository
    {
        public IEnumerable<Employee> GetAllEmployee(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var employees = multi.Read<Employee, JobTitle, Group, Employee>(
                           (emp, jb, gr) =>
                           {
                               emp.JobTitle = jb;
                               emp.Group = gr;

                               return emp;
                           },
                           "JobTitleId, GroupId");

                    var roles = multi.Read<EmployeeRoleModel>();

                    foreach (Employee employee in employees)
                    {
                        IList<SystemRoleModel> employeeRoles = roles.Where(role => role.EmployeeId == employee.EmployeeId)
                            .Select(role => new SystemRoleModel
                            {
                                RoleId = role.RoleId,
                                RoleName = role.RoleName,
                                RoleCode = role.RoleCode
                            }).ToList();

                        employee.Roles = employeeRoles;
                    }

                    return employees;
                }
            }
        }

        public UserSession GetEmployeeLogin(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var user = multi.Read<UserSession>().FirstOrDefault();

                    if (user != null && user.EmployeeId > 0)
                    {
                        user.Roles = multi.Read<SystemRoleModel>();
                        user.Functions = multi.Read<FunctionModel>();
                    }

                    return user;
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
                        employee.Roles = multi.Read<SystemRoleModel>().ToList();
                    }

                    return employee;
                }
            }
        }
    }
}