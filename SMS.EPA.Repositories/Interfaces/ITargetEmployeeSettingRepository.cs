namespace SMS.EPA.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using SMS.EPA.Model.TargetEmployeeSetting;

    public interface ITargetEmployeeSettingRepository
    {
        TargetEmployeeSettingModel GetTargetEmployeeSetting(ICriteria criteria);

        TargetDepartmentSettingModel GetTargetDepartmentSetting(ICriteria criteria);
    }
}