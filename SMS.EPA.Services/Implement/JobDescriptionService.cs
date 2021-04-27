namespace SMS.EPA.Services
{
    using System.Threading.Tasks;
    using SMS.EPA.Criteria.JobDescription;
    using SMS.EPA.Model.JobDescription;
    using SMS.EPA.Repositories;

    public class JobDescriptionService : IJobDescriptionService
    {
        private readonly IJobDescriptionRepository repository;

        public JobDescriptionService() : this(null)
        {
        }

        public JobDescriptionService(IJobDescriptionRepository repository)
        {
            this.repository = repository ?? new JobDescriptionRepository();
        }

        public async Task<JobDescriptionEmployeeSetting> GetEmployeeSettingByIdAsync(int employeeId)
        {
            var criteria = new GetEmployeeSettingJobDescriptionByIdCriteria { EmployeeId = employeeId };
            return await repository.GetEmployeeSettingByIdAsync(criteria);
        }
    }
}