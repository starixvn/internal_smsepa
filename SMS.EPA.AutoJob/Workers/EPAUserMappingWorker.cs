namespace SMS.EPA.AutoJob.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using SMS.EPA.AutoJob.Constants;
    using SMS.EPA.AutoJob.Extensions;
    using SMS.EPA.AutoJob.Models;
    using SMS.EPA.AutoJob.Service;

    public class EPAUserMappingWorker : BaseWorker
    {
        private readonly IEPAUserMappingService epaUserMappingService;

        public EPAUserMappingWorker(
            IEPAUserMappingService epaUserMappingService,
            IOptions<List<JobSetting>> jobSettings
        )
        {
            this.epaUserMappingService = epaUserMappingService;
            jobSetting = JobHelper.GetJobConfig(jobSettings.Value, JobConstants.SyncEPAUserControlMapping);
        }

        public override async Task ExecuteJobAsync()
        {
            await epaUserMappingService.SyncDataUserMapping();
        }
    }
}
