namespace SMS.EPA.ClientHrm
{
    using Fanex.Logging;
    using SMS.EPA.ClientHrm.Services;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.ServiceProcess;
    using System.Timers;

    [RunInstaller(true)]
    public partial class WinService : ServiceBase
    {
        private ISynchronizeService _service;
        private bool _isWorking = false;
        private Timer _timer;

        public WinService(ISynchronizeService service)
        {
            InitializeComponent();
            _service = service;
            _timer = new Timer(ApplicationConfig.Interval);
            _timer.Elapsed += new ElapsedEventHandler(ExecuteJob);
            _timer.Enabled = false;
        }

        protected override void OnStart(string[] args)
        {
            _isWorking = true;
            _timer.Enabled = true;
        }

        protected override void OnStop()
        {
            _isWorking = false;
            _timer.Enabled = false;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "OK")]
        private void ExecuteJob(object sender, ElapsedEventArgs e)
        {
            if (_isWorking)
            {
                _isWorking = false;
                try
                {
                    _service.Synchronize();
                }
                catch (Exception ex)
                {
                    LogService.Error(ex.Message);
                }

                _isWorking = true;
            }
        }
    }
}