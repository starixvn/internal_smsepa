namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.TargetEmployeeSetting;
    using SMS.EPA.Model.TargetEmployeeSetting;

    public interface ITargetEmployeeSettingService
    {
        IList<TargetEmployeeModel> GetTargetEmployeeSetting(GetAllTargetEmployeeSettingCriteria criteria);

        CommandResult AssignTargetTemplateToEmployee(AssignTargetTemplateToEmployeeCriteria criteria);

        CommandResult DeleteTargetEmployeeSetting(DeleteTargetEmployeeSettingCriteria criteria);

        IList<TargetDepartmentModel> GetTargetDepartmentSetting(GetTargetDepartmentSettingCriteria criteria);

        CommandResult AssignTargetTemplateToDepartment(AssignTargetDepartmentSettingCriteria criteria);

        CommandResult DeleteTargetDepartmentSetting(DeleteTargetDepartmentSettingCriteria criteria);
    }
}