namespace SMS.EPA.ClientHRM.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Quartz;
    using SMS.EPA.ClientHrm.Services;
    using SMS.EPA.ClientHRM.Model.UnitHead;

    public class SyncUnitHeadScheduleJob : IJob
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(SyncUnitHeadScheduleJob));

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var dataMap = context.JobDetail.JobDataMap;
                var service = (IUnitHeadService)dataMap.Get("unitHeadService");

                IEnumerable<UnitHeadSchedule> schedules = await service.GetUnitHeadScheduleAsync();

                if (schedules != null && schedules.Any())
                {
                    logger.Info("begin");
                    await service.UpdateUnitHeadScheduleAsync(schedules);
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