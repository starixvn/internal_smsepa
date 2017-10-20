namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using Core.Services;
    using Criteria;
    using Interfaces;
    using Model;
    using Repositories;

    public class TargetEmployeeSettingService : BaseService, ITargetEmployeeSettingService
    {
        private readonly ITargetEmployeeSettingRepository targetEmployeeSettingRepository;

        public TargetEmployeeSettingService() : this(null)
        {
        }

        public TargetEmployeeSettingService(ITargetEmployeeSettingRepository targetEmployeeSettingRepository)
        {
            this.targetEmployeeSettingRepository = targetEmployeeSettingRepository ?? new TargetEmployeeSettingRepository();
        }

        public IList<TargetEmployeeSetting> GetTargetEmployeeSetting(GetAllTargetEmployeeSettingCriteria criteria)
        {
            IList<TargetEmployeeSetting> targetEmployees = null;

            if (criteria != null && criteria.IsValid())
            {
                targetEmployees = targetEmployeeSettingRepository.GetTargetTemplateEmployee(criteria);
            }

            return targetEmployees ?? new List<TargetEmployeeSetting>();
        }

        public IList<TargetDepartmentSetting> GetTargetDepartmentSetting(GetTargetDepartmentSettingCriteria criteria)
        {
            IList<TargetDepartmentSetting> targetDepartments = null;

            if (criteria != null && criteria.IsValid())
            {
                targetDepartments = targetEmployeeSettingRepository.GetTargetTemplateDepartment(criteria);
            }

            return targetDepartments ?? new List<TargetDepartmentSetting>();
        }
    }
}