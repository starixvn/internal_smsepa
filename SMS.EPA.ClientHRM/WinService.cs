namespace SMS.EPA.ClientHrm
{
    using System;
    using System.ComponentModel;
    using System.ServiceProcess;
    using SMS.EPA.ClientHrm.Services;
    using SMS.EPA.ClientHRM.Jobs;

    [RunInstaller(true)]
    public partial class WinService : ServiceBase
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(WinService));

        private readonly ISyncEPADataService epaService;
        private readonly IUserMappingService userMappingService;
        private readonly IUnitHeadService unitHeadService;

        private readonly JobScheduler scheduler;

        public WinService(
            ISyncEPADataService epaService,
            IUserMappingService userMappingService,
            IUnitHeadService unitHeadService)
        {
            InitializeComponent();

            this.epaService = epaService;
            this.userMappingService = userMappingService;
            this.unitHeadService = unitHeadService;

            scheduler = new JobScheduler();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                logger.Info("HRMService: OnStart");

                scheduler.Start(epaService, userMappingService, unitHeadService);
            }
            catch (Exception ex)
            {
                logger.Error("HRMService: OnStart -- " + ex.Message, ex);
            }
        }

        protected override void OnStop()
        {
            try
            {
                logger.Info("HRMService: OnStop");
                scheduler.Stop();
            }
            catch (Exception ex)
            {
                logger.Error("HRMService: OnStop -- " + ex.Message, ex);
            }
        }
    }
}