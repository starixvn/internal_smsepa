namespace SMS.EPA.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using SMS.EPA.Model.TargetEmployeeSetting;
    using System.Collections.Generic;

    public interface ITargetEmployeeSettingRepository
    {
        IList<TargetEmployee> GetTargetEmployee(ICriteria criteria);

        TargetEmployeeSettingModel GetTargetEmployeeSetting(ICriteria criteria);

        TargetDepartmentSettingModel GetTargetDepartmentSetting(ICriteria criteria);
    }
}