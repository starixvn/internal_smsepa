namespace SMS.EPA.ClientHRM.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Quartz;
    using SMS.EPA.ClientHrm.Services;
    using SMS.EPA.ClientHRM.Model.UnitHead;

    public class SyncUnitHeadInfoJob : IJob
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(SyncUnitHeadInfoJob));

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var dataMap = context.JobDetail.JobDataMap;
                var service = (IUnitHeadService)dataMap.Get("unitHeadService");

                IEnumerable<UnitHeadInfo> infos = await service.GetUnitHeadInfoAsync();

                if (infos != null && infos.Any())
                {
                    await service.UpdateUnitHeadInfoAsync(infos);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }
    }
}