namespace SMS.EPA.Services
{
    using System.Threading.Tasks;
    using SMS.EPA.Model.JobDescription;

    public interface IJobDescriptionService
    {
        Task<JobDescriptionGroupSetting> GetGroupSettingByIdAsync(int groupId, int jobLevelId);

        Task<JobDescriptionEmployeeSetting> GetEmployeeSettingByIdAsync(int employeeId);
    }
}