namespace SMS.EPA.ClientHrm
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.ServiceProcess;
    using System.Timers;
    using Services;

    [RunInstaller(true)]
    public partial class WinService : ServiceBase
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(WinService));
        private readonly ISynchronizeService _service;
        private readonly Timer _timer;

        private bool _isWorking;

        public WinService(ISynchronizeService service)
        {
            InitializeComponent();
            _service = service;
            _timer = new Timer { Interval = ApplicationConfig.Interval };
            _timer.Elapsed += ExecuteJob;
            _timer.Enabled = false;
        }

        protected override void OnStart(string[] args)
        {
            _timer.Enabled = true;
        }

        protected override void OnStop()
        {
            _timer.Enabled = false;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "OK")]
        private void ExecuteJob(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (!_isWorking)
                {
                    _isWorking = true;
                    _service.Synchronize();
                    _isWorking = false;
                }
            }
            catch (Exception ex)
            {
                _isWorking = false;
                logger.Error(ex.Message);
            }
        }
    }
}