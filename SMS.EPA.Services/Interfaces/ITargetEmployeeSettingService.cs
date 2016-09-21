namespace SMS.EPA.Services.Interfaces
{
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.TargetEmployeeSetting;
    using SMS.EPA.Model;
    using System.Collections.Generic;

    public interface ITargetEmployeeSettingService
    {
        IList<TargetEmployeeSetting> GetTargetEmployeeSetting(GetAllTargetEmployeeSettingCriteria criteria);

        IList<TargetDepartmentSetting> GetTargetDepartmentSetting(GetTargetDepartmentSettingCriteria criteria);

        CommandResult AssignTargetTemplateToEmployee(AssignTargetTemplateToEmployeeCriteria criteria);

        CommandResult DeleteTargetEmployeeSetting(DeleteTargetEmployeeSettingCriteria criteria);

        CommandResult AssignTargetTemplateToDepartment(AssignTargetDepartmentSettingCriteria criteria);

        CommandResult DeleteTargetDepartmentSetting(DeleteTargetDepartmentSettingCriteria criteria);
    }
}