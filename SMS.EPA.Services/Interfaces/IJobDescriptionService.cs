namespace SMS.EPA.Services
{
    using SMS.EPA.Model.JobDescription;

    public interface IJobDescriptionService
    {
        JobDescriptionGroupSetting GetGroupSettingById(int groupId, int jobLevelId);

        JobDescriptionEmployeeSetting GetEmployeeSettingById(int employeeId);
    }
}