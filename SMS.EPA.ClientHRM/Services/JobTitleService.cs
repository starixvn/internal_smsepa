namespace SMS.EPA.ClientHrm.Services
{
    using Core.Model;
    using Criteria;
    using Fanex.Data.Repository;
    using Model;
    using System.Collections.Generic;
    using ThirdPartyAPI;

    public class JobTitleService : IJobTitleService
    {
        private readonly IDynamicRepository _dynamicRepository;
        private readonly IJobTitleAPI _jobTitleApi;

        public JobTitleService(IDynamicRepository dynamicRepository, IJobTitleAPI jobTitleApi)
        {
            _dynamicRepository = dynamicRepository;
            _jobTitleApi = jobTitleApi;
        }

        public IEnumerable<JobTitle> GetJobTitles()
        {
            var criteria = new GetJobTitleCriteria();
            IEnumerable<JobTitle> jobTitles = _dynamicRepository.Fetch<JobTitle>(criteria);

            return jobTitles;
        }

        public CommandResult Update(IEnumerable<JobTitle> jobTitles, TokenAccess token)
        {
            return _jobTitleApi.Update(jobTitles, token);
        }
    }
}