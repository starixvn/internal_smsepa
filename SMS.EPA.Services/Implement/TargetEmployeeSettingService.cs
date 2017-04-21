namespace SMS.EPA.Services.Implement
{
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Interfaces;
    using Model;
    using Repositories;
    using System.Collections.Generic;

    public class TargetEmployeeSettingService : BaseService, ITargetEmployeeSettingService
    {
        private readonly ITargetEmployeeSettingRepository _targetEmployeeSettingRepository;

        public TargetEmployeeSettingService()
            : this(null)
        {
        }

        public TargetEmployeeSettingService(ITargetEmployeeSettingRepository targetEmployeeSettingRepository)
        {
            _targetEmployeeSettingRepository = targetEmployeeSettingRepository ?? new TargetEmployeeSettingRepository();
        }

        public IList<TargetEmployeeSetting> GetTargetEmployeeSetting(GetAllTargetEmployeeSettingCriteria criteria)
        {
            IList<TargetEmployeeSetting> targetEmployees = null;

            if (criteria != null && criteria.IsValid())
            {
                targetEmployees = _targetEmployeeSettingRepository.GetTargetTemplateEmployee(criteria);
            }

            return targetEmployees ?? new List<TargetEmployeeSetting>();
        }

        public IList<TargetDepartmentSetting> GetTargetDepartmentSetting(GetTargetDepartmentSettingCriteria criteria)
        {
            IList<TargetDepartmentSetting> targetDepartments = null;

            if (criteria != null && criteria.IsValid())
            {
                targetDepartments = _targetEmployeeSettingRepository.GetTargetTemplateDepartment(criteria);
            }

            return targetDepartments ?? new List<TargetDepartmentSetting>();
        }

        public CommandResult AssignTargetTemplateToEmployee(AssignTargetTemplateToEmployeeCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult DeleteTargetEmployeeSetting(DeleteTargetEmployeeSettingCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Delete(criteria);
            }

            return commandResult;
        }

        public CommandResult AssignTargetTemplateToDepartment(AssignTargetDepartmentSettingCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult DeleteTargetDepartmentSetting(DeleteTargetDepartmentSettingCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Delete(criteria);
            }

            return commandResult;
        }
    }
}