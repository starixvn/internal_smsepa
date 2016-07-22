namespace SMS.EPA.ClientHRM
{
    using Fanex.Logging;
    using SMS.EPA.ClientHRM.Model;
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
            LogService.Info("start");
            _isWorking = true;
            _timer.Enabled = true;
        }

        protected override void OnStop()
        {
            LogService.Info("stop");
            _isWorking = false;
            _timer.Enabled = false;
        }

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
                    var message = ex.Message + Environment.NewLine +
                        "StackTrace: " + ex.StackTrace;
                    if (ex is WebApiException)
                    {
                        var customException = ex as WebApiException;
                        message = "Status Code: " + customException.StatusCode + Environment.NewLine;
                        message += "Http Request Message: " + customException.RequestMessage + Environment.NewLine;
                        message += "Data :" + customException.AttachedData + Environment.NewLine;
                    }

                    LogService.Error(message);
                }

                _isWorking = true;
            }
        }
    }
}
