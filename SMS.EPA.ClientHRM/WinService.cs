using Fanex.Logging;
using SMS.EPA.ClientHRM.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;

namespace SMS.EPA.ClientHRM
{
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

                    _service.SynchronizeAll();

                    _isWorking = true;
                }
            }
            catch (Exception ex)
            {
                _isWorking = false;
                LogService.Error(ex.Message);
            }
        }
    }
}
