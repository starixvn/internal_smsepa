namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;

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