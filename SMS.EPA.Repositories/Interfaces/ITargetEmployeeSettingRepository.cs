namespace SMS.EPA.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using SMS.EPA.Model;
    using System.Collections.Generic;

    public interface ITargetEmployeeSettingRepository
    {
        IList<TargetEmployeeSetting> GetTargetTemplateEmployee(ICriteria criteria);

        IList<TargetDepartmentSetting> GetTargetTemplateDepartment(ICriteria criteria);
    }
}