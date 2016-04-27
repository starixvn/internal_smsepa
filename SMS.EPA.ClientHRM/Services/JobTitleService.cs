using Fanex.Data.Repository;
using SMS.EPA.ClientHRM.Criteria;
using SMS.EPA.ClientHRM.Model;
using SMS.EPA.ClientHRM.ThirdPartyAPIs;
using System;
using System.Collections.Generic;

namespace SMS.EPA.ClientHRM.Services
{
    public class JobTitleService : IJobTitleService
    {
        private readonly IDynamicRepository _dynamicRepository;
        private readonly IJobTitleAPI _jobTitleAPI;

        public JobTitleService() : this(null, null) 
        {
        }

        public JobTitleService(IDynamicRepository dynamicRepository, IJobTitleAPI jobTitleAPI)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
            _jobTitleAPI = jobTitleAPI ?? new JobTitleAPI();
        }

        public IEnumerable<JobTitle> GetJobTitles()
        {
            var criteria = new GetJobTitleCriteria();
            IEnumerable<JobTitle> jobTitles = _dynamicRepository.Fetch<JobTitle>(criteria);

            return jobTitles;
        }

        public CommandResult Update(IEnumerable<JobTitle> jobTitles, TokenAccess token)
        {
            return _jobTitleAPI.Update(jobTitles, token);
        }
    }
}
