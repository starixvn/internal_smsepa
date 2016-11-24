namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;

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

        private IList<TargetEmployeeSetting> MappingTargetTemplateEmployee(
            IEnumerable<Employee> employees,
            IEnumerable<TargetTemplateEmployee> targetTemplateEmployees)
        {
            var targetEmployees = new List<TargetEmployeeSetting>();
            IList<TargetTemplateEmployee> tempTargetTemplateEmployees;

            foreach (Employee employee in employees)
            {
                tempTargetTemplateEmployees = targetTemplateEmployees.Where(t => t.EmployeeId == employee.EmployeeId).ToList();
                targetEmployees.Add(new TargetEmployeeSetting()
                {
                    Employee = employee,
                    TargetTemplates = tempTargetTemplateEmployees
                });
            }

            return targetEmployees;
        }

        private IList<TargetDepartmentSetting> MappingTargetTemplateDepartment(
            IEnumerable<Department> departments,
            IEnumerable<TargetTemplateDepartment> targetTemplateDepartments)
        {
            var targetDepartments = new List<TargetDepartmentSetting>();
            IList<TargetTemplateDepartment> tempTargetTemplateDepartments;

            foreach (Department department in departments)
            {
                tempTargetTemplateDepartments = targetTemplateDepartments.Where(
                    t => t.DepartmentId == department.DepartmentId && t.IsGroup == department.IsGroup).ToList();
                targetDepartments.Add(new TargetDepartmentSetting()
                {
                    Department = department,
                    TargetTemplates = tempTargetTemplateDepartments
                });
            }

            return targetDepartments;
        }
    }
}