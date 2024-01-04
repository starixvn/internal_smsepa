namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Interfaces;

    public class TargetEmployeeSettingService : ITargetEmployeeSettingService
    {
        private readonly IBaseService baseService;

        public TargetEmployeeSettingService() : this(null)
        {
        }

        public TargetEmployeeSettingService(IBaseService baseService)
        {
            this.baseService = baseService ?? new BaseService();
        }

        public IList<TargetEmployeeSetting> GetTargetEmployeeSetting(GetAllTargetEmployeeSettingCriteria criteria)
        {
            (var employees, var templates) = baseService.FetchMultiple<TargetEmployeeSetting, TargetTemplateEmployee>(criteria);
            foreach (var employee in employees)
            {
                employee.TargetTemplates = templates.Where(t => t.EmployeeId == employee.EmployeeId).ToList();
            }

            return employees.ToList();
        }

        public IList<TargetDepartmentSetting> GetTargetDepartmentSetting(GetTargetDepartmentSettingCriteria criteria)
        {
            (var departments, var templates) = baseService.FetchMultiple<TargetDepartmentSetting, TargetTemplateDepartment>(criteria);
            foreach (var department in departments)
            {
                department.TargetTemplates = templates.Where(t => t.DepartmentId == department.DepartmentId && t.IsGroup == department.IsGroup).ToList();
            }

            return departments.ToList();
        }
    }
}