namespace SMS.EPA.ClientHrm.Services
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.ClientHrm.Criteria;
    using SMS.EPA.ClientHrm.Model;
    using SMS.EPA.ClientHrm.ThirdPartyAPIs;

    public class JobTitleService : IJobTitleService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IJobTitleAPI jobTitleAPI;

        public JobTitleService(IDynamicRepository dynamicRepository, IJobTitleAPI jobTitleAPI)
        {
            this.dynamicRepository = dynamicRepository;
            this.jobTitleAPI = jobTitleAPI;
        }

        public IEnumerable<JobTitle> GetJobTitles()
        {
            var criteria = new GetJobTitleCriteria();
            IEnumerable<JobTitle> jobTitles = dynamicRepository.Fetch<JobTitle>(criteria);

            return jobTitles;
        }

        public CommandResult Update(IEnumerable<JobTitle> jobTitles, TokenAccess token)
        {
            return jobTitleAPI.Update(jobTitles, token);
        }
    }
}