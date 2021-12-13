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
            var tuple = baseService.FetchMultiple<TargetEmployeeSetting, TargetTemplateEmployee>(criteria);
            foreach (TargetEmployeeSetting employee in tuple.Item1)
            {
                employee.TargetTemplates = tuple.Item2.Where(t => t.EmployeeId == employee.EmployeeId).ToList();
            }

            return tuple.Item1.ToList();
        }

        public IList<TargetDepartmentSetting> GetTargetDepartmentSetting(GetTargetDepartmentSettingCriteria criteria)
        {
            var tuple = baseService.FetchMultiple<TargetDepartmentSetting, TargetTemplateDepartment>(criteria);
            foreach (TargetDepartmentSetting department in tuple.Item1)
            {
                department.TargetTemplates = tuple.Item2.Where(t => t.DepartmentId == department.DepartmentId && t.IsGroup == department.IsGroup).ToList();
            }

            return tuple.Item1.ToList();
        }
    }
}