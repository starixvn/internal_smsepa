namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Repositories.Interfaces;

    public class EmployeeRepository : IEmployeeRepository
    {
        public IEnumerable<EmployeeModel> GetAllEmployee(ICriteria criteria)
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

                    IEnumerable<EmployeeRoleModel> roles = multi.Read<EmployeeRoleModel>();

                    foreach (EmployeeModel employee in employees)
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
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    UserSession user = multi.Read<UserSession>().FirstOrDefault();

                    if (user != null && user.EmployeeId > 0)
                    {
                        user.Roles = multi.Read<SystemRoleModel>();
                        user.Functions = multi.Read<FunctionModel>();
                    }

                    return user;
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