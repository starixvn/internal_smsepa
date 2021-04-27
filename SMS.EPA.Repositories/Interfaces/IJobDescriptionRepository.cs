namespace SMS.EPA.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.JobDescription;

    public interface IJobDescriptionRepository
    {
        Task<JobDescriptionEmployeeSetting> GetEmployeeSettingByIdAsync(ICriteria criteria);
    }
}