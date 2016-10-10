namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Model;

    public interface ITargetEmployeeSettingRepository
    {
        IList<TargetEmployeeSetting> GetTargetTemplateEmployee(ICriteria criteria);

        IList<TargetDepartmentSetting> GetTargetTemplateDepartment(ICriteria criteria);
    }
}