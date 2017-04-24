namespace SMS.EPA.Repositories
{
    using Fanex.Data.Repository;
    using Model;
    using System.Collections.Generic;

    public interface ITargetEmployeeSettingRepository
    {
        IList<TargetEmployeeSetting> GetTargetTemplateEmployee(ICriteria criteria);

        IList<TargetDepartmentSetting> GetTargetTemplateDepartment(ICriteria criteria);
    }
}