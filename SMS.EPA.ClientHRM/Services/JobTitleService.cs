namespace SMS.EPA.ClientHrm.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Model;
    using Criteria;
    using EPA.Model.API;
    using Fanex.Data.Repository;
    using Model;
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

        public async Task<CommandResult> UpdateAsync(IEnumerable<JobTitle> jobTitles, TokenAccess token)
        {
            return await _jobTitleApi.UpdateAsync(jobTitles, token);
        }
    }
}