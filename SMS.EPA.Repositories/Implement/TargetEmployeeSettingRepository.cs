namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;
    using Model.Enitites;

    public class TargetEmployeeSettingRepository : ITargetEmployeeSettingRepository
    {
        public IList<TargetEmployeeSetting> GetTargetTemplateEmployee(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var employees = multi.Read<Employee, Group, Employee>(
                        (emp, gr) =>
                        {
                            emp.Group = gr;
                            return emp;
                        },
                        "GroupId");

                    var targetEmployees = multi.Read<TargetTemplateEmployee>();

                    return MappingTargetTemplateEmployee(employees, targetEmployees);
                }
            }
        }

        public IList<TargetDepartmentSetting> GetTargetTemplateDepartment(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var departments = multi.Read<TargetDepartmentSetting>();
                    var templates = multi.Read<TargetTemplateDepartment>();

                    foreach (TargetDepartmentSetting department in departments)
                    {
                        department.TargetTemplates = templates.Where(t => t.DepartmentId == department.DepartmentId && t.IsGroup == department.IsGroup).ToList();
                    }

                    return departments.ToList();
                }
            }
        }

        private static IList<TargetEmployeeSetting> MappingTargetTemplateEmployee(
            IEnumerable<Employee> employees,
            IEnumerable<TargetTemplateEmployee> targetTemplateEmployees)
        {
            var targetEmployees = new List<TargetEmployeeSetting>();

            foreach (Employee employee in employees)
            {
                IList<TargetTemplateEmployee> tempTargetTemplateEmployees =
                    targetTemplateEmployees.Where(t => t.EmployeeId == employee.EmployeeId).ToList();
                targetEmployees.Add(new TargetEmployeeSetting
                {
                    Employee = employee,
                    TargetTemplates = tempTargetTemplateEmployees
                });
            }

            return targetEmployees;
        }
    }
}