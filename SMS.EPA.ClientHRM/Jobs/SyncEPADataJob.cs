namespace SMS.EPA.ClientHRM.Jobs
{
    using System;
    using System.Threading.Tasks;
    using Quartz;
    using SMS.EPA.ClientHrm.Services;

    public class SyncEPADataJob : IJob
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(SyncEPADataJob));

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var dataMap = context.JobDetail.JobDataMap;
                var service = (ISyncEPADataService)dataMap.Get("epaService");

                await service.ExecuteAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }
    }
}