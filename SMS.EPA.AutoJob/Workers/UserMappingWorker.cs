namespace SMS.EPA.AutoJob.Workers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using SMS.EPA.AutoJob.Constants;
    using SMS.EPA.AutoJob.Extensions;
    using SMS.EPA.AutoJob.Models;
    using SMS.EPA.AutoJob.Services;

    public class UserMappingWorker : BaseWorker
    {
        private readonly IUserMappingService userMappingService;

        public UserMappingWorker(
            IUserMappingService userMappingService,
            List<JobSettings> jobSettings,
            ILogger<UserMappingWorker> logger) : base(logger)
        {
            this.userMappingService = userMappingService;
            jobSetting = JobHelper.GetJobConfig(jobSettings, JobConstants.SyncUserMapping);
        }

        public override async Task DoWorkAsync()
        {
            await userMappingService.SyncUserMappingAsync();
        }
    }
}