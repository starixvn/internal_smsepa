namespace SMS.EPA.ClientHRM.Jobs
{
    using System.Collections.Generic;
    using Quartz;
    using Quartz.Impl;
    using SMS.EPA.ClientHrm;
    using SMS.EPA.ClientHrm.Services;

    public class JobScheduler
    {
        private readonly int interval;
        private readonly IScheduler scheduler;

        public JobScheduler()
        {
            interval = ApplicationConfig.Interval;

            var factory = new StdSchedulerFactory();

            scheduler = factory.GetScheduler().Result;
        }

        public void Start(ISyncEPADataService epaService, IUserMappingService userMappingService, IUnitHeadService unitHeadService)
        {
            // sync epa data
            var epaJob = JobBuilder.Create<SyncEPADataJob>()
                .UsingJobData(new JobDataMap(new Dictionary<string, ISyncEPADataService>
                {
                    { nameof(epaService), epaService }
                }))
                .Build();

            var epaTrigger = TriggerBuilder.Create()
                   .WithSimpleSchedule(a => a.WithIntervalInMinutes(interval)
                   .RepeatForever())
                   .Build();

            scheduler.ScheduleJob(epaJob, epaTrigger);

            if (ApplicationConfig.IsEnableSyncUserMapping)
            {
                var userMappingJob = JobBuilder.Create<SyncUserMappingJob>()
                    .UsingJobData(new JobDataMap(new Dictionary<string, IUserMappingService>
                    {
                    { nameof(userMappingService), userMappingService }
                    }))
                    .Build();

                var userMappingTrigger = TriggerBuilder.Create()
                       .WithSimpleSchedule(a => a.WithIntervalInMinutes(interval)
                       .RepeatForever())
                       .Build();

                scheduler.ScheduleJob(userMappingJob, userMappingTrigger);
            }

            if (ApplicationConfig.IsEnableSyncUnitHead)
            {
                // sync unit head info
                var unitHeadInfoJob = JobBuilder.Create<SyncUnitHeadInfoJob>()
                    .UsingJobData(new JobDataMap(new Dictionary<string, IUnitHeadService>
                    {
                    { nameof(unitHeadService), unitHeadService }
                    }))
                    .Build();

                var unitHeadInfoTrigger = TriggerBuilder.Create()
                       .WithSimpleSchedule(a => a.WithIntervalInMinutes(interval)
                       .RepeatForever())
                       .Build();

                // sync unit head schedule
                var unitHeadScheduleJob = JobBuilder.Create<SyncUnitHeadScheduleJob>()
                    .UsingJobData(new JobDataMap(new Dictionary<string, IUnitHeadService>
                    {
                    { nameof(unitHeadService), unitHeadService }
                    }))
                    .Build();

                var unitHeadScheduleTrigger = TriggerBuilder.Create()
                       .WithSimpleSchedule(a => a.WithIntervalInMinutes(interval)
                       .RepeatForever())
                       .Build();

                scheduler.ScheduleJob(unitHeadInfoJob, unitHeadInfoTrigger);
                scheduler.ScheduleJob(unitHeadScheduleJob, unitHeadScheduleTrigger);
            }

            scheduler.Start().Wait();
        }

        public void Stop()
        {
            scheduler.Shutdown().Wait();
        }
    }
}