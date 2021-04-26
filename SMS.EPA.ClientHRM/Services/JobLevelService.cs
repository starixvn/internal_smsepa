namespace SMS.EPA.ClientHrm.Services
{
    using Core.Model;
    using Criteria;
    using EPA.Model.API;
    using Fanex.Data.Repository;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ThirdPartyAPI;

    public class JobLevelService : IJobLevelService
    {
        private readonly IDynamicRepository _dynamicRepository;
        private readonly IJobLevelAPI jobLevelApi;

        public JobLevelService(IDynamicRepository dynamicRepository, IJobLevelAPI jobLevelApi)
        {
            _dynamicRepository = dynamicRepository;
            this.jobLevelApi = jobLevelApi;
        }

        public IEnumerable<JobLevel> GetJobLevel(DateTime? fromDate)
        {
            var criteria = new GetJobLevelCriteria { FromDate = fromDate };
            IEnumerable<JobLevel> jobLevel = _dynamicRepository.Fetch<JobLevel>(criteria);

            return jobLevel;
        }

        public async Task<DateTime> GetLastModifiedDateAsync(TokenAccess token)
        {
            return await jobLevelApi.GetLastModifiedDate(token);
        }

        public Task<CommandResult> Update(IEnumerable<JobLevel> jobLevel, TokenAccess token)
        {
            return jobLevelApi.UpdateAsync(jobLevel, token);
        }
    }
}