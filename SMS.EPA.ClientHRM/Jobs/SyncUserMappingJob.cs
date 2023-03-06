namespace SMS.EPA.ClientHRM.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Quartz;
    using SMS.EPA.ClientHrm.Services;
    using SMS.EPA.Model.API;

    public class SyncUserMappingJob : IJob
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(SyncUserMappingJob));

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var dataMap = context.JobDetail.JobDataMap;
                var service = (IUserMappingService)dataMap.Get("userMappingService");

                DateTime? lastModifiedDate = await service.GetLastModifiedDateAsync();
                IEnumerable<UserMapping> userMappings = await service.GetUserMappingAsync(lastModifiedDate);

                if (userMappings != null && userMappings.Any())
                {
                    logger.Info("begin");
                    await service.UpdateUserMappingAsync(userMappings);
                    logger.Info("end");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }
    }
}