namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Core.Model;
    using Criteria;
    using Model;

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