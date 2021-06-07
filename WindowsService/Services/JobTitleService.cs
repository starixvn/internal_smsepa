namespace SMS.EPA.WindowsService.Services
{
    using Core.Model;
    using Criteria;
    using EPA.Model.API;
    using Fanex.Data.Repository;
    using Model;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ThirdPartyAPI;

    public class JobTitleService : IJobTitleService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IJobTitleAPI jobTitleApi;

        public JobTitleService(IDynamicRepository dynamicRepository, IJobTitleAPI jobTitleApi)
        {
            this.dynamicRepository = dynamicRepository;
            this.jobTitleApi = jobTitleApi;
        }

        public IEnumerable<JobTitle> GetJobTitles()
        {
            var criteria = new GetJobTitleCriteria();
            IEnumerable<JobTitle> jobTitles = dynamicRepository.Fetch<JobTitle>(criteria);

            return jobTitles;
        }

        public async Task<CommandResult> UpdateAsync(IEnumerable<JobTitle> jobTitles, TokenAccess token)
        {
            return await jobTitleApi.UpdateAsync(jobTitles, token);
        }
    }
}