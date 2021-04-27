namespace SMS.EPA.Services
{
    using System.Threading.Tasks;
    using SMS.EPA.Model.JobDescription;

    public interface IJobDescriptionService
    {
        Task<JobDescriptionEmployeeSetting> GetEmployeeSettingByIdAsync(int employeeId);
    }
}