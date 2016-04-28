namespace SMS.EPA.ClientHRM
{
    using Fanex.Logging;
    using SMS.EPA.ClientHRM.Services;
    using System;
    using System.ComponentModel;
    using System.ServiceProcess;
    using System.Timers;

    [RunInstaller(true)]
    public partial class WinService : ServiceBase
    {
        private ISynchronizeService _service;
        private bool _isWorking = false;
        private Timer _timer;

        public WinService()
        {
            InitializeComponent();
            _service = new SynchronizeService();

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

        private void ExecuteJob(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (_isWorking)
                {
                    _isWorking = false;

                    _service.Synchronize();

                    _isWorking = true;
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message + Environment.NewLine +
                    "StackTrace: " + ex.StackTrace;

                LogService.Error(message);
            }
        }
    }
}
