namespace SMS.EPA.Repositories
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;
    using Model.Enitites;
    using System.Collections.Generic;
    using System.Linq;

    public class TargetEmployeeSettingRepository : ITargetEmployeeSettingRepository
    {
        public IList<TargetEmployeeSetting> GetTargetTemplateEmployee(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReaderWrapper multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<Employee> employees = multi.Read<Employee, Group, Employee>(
                        (emp, gr) =>
                        {
                            emp.Group = gr;
                            return emp;
                        },
                        "GroupId");

                    IEnumerable<TargetTemplateEmployee> targetEmployees = multi.Read<TargetTemplateEmployee>();

                    return MappingTargetTemplateEmployee(employees, targetEmployees);
                }
            }
        }

        public IList<TargetDepartmentSetting> GetTargetTemplateDepartment(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReaderWrapper multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<Department> deparments = multi.Read<Department>();

                    IEnumerable<TargetTemplateDepartment> targetDepartments = multi.Read<TargetTemplateDepartment>();

                    return MappingTargetTemplateDepartment(deparments, targetDepartments);
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
                IList<TargetTemplateEmployee> tempTargetTemplateEmployees = targetTemplateEmployees
                    .Where(t => t.EmployeeId == employee.EmployeeId)
                    .ToList();
                targetEmployees.Add(new TargetEmployeeSetting
                {
                    Employee = employee,
                    TargetTemplates = tempTargetTemplateEmployees
                });
            }

            return targetEmployees;
        }

        private static IList<TargetDepartmentSetting> MappingTargetTemplateDepartment(
            IEnumerable<Department> departments,
            IEnumerable<TargetTemplateDepartment> targetTemplateDepartments)
        {
            var targetDepartments = new List<TargetDepartmentSetting>();

            foreach (Department department in departments)
            {
                IList<TargetTemplateDepartment> tempTargetTemplateDepartments = targetTemplateDepartments.Where(
                        t => t.DepartmentId == department.DepartmentId && t.IsGroup == department.IsGroup)
                    .ToList();
                targetDepartments.Add(new TargetDepartmentSetting
                {
                    Department = department,
                    TargetTemplates = tempTargetTemplateDepartments
                });
            }

            return targetDepartments;
        }
    }
}