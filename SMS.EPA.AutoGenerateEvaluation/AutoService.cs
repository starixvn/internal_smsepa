// ----------------------------------------------------------------------
// <copyright file="AutoService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/27/2015">Created.</change>
// </history>

namespace SMS.EPA.AutoGenerateEvaluation
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Reflection;
    using System.ServiceProcess;
    using System.Timers;
    using log4net;
    using SMS.EPA.AutoGenerateEvaluation.Service;
    using SMS.EPA.Core.Model;

    public partial class AutoService : ServiceBase
    {
        private static readonly ILog _LOG = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool _isBusy = false;
        private Timer _updateCounterTimer = null;
        private GenerationEvaluationService _generationEvaluationService;

        public AutoService()
        {
            InitializeComponent();

            TimerInterval = int.Parse(ConfigurationManager.AppSettings["TimerInterval"]);
            _updateCounterTimer = new Timer(TimerInterval * 1000);
            _updateCounterTimer.Elapsed += new ElapsedEventHandler(ExecuteJob);
            _updateCounterTimer.Enabled = false;

            _generationEvaluationService = new GenerationEvaluationService();
        }

        public int TimerInterval { get; private set; }

        protected override void OnStart(string[] args)
        {
            _LOG.Info("Service started at " + DateTime.Now.ToString());
            _updateCounterTimer.Enabled = true;
        }

        protected override void OnStop()
        {
            _updateCounterTimer.Enabled = false;
            _LOG.Info("Service stopped at " + DateTime.Now.ToString());
        }

        private void ExecuteJob(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (!_isBusy)
                {
                    _isBusy = true;
                    CheckAddEmployeeAndGenerateEvaluation();
                    _isBusy = false;
                }
            }
            catch (Exception ex)
            {
                _isBusy = false;
                _LOG.Error("Error: " + ex.InnerException.Message);
            }
        }

        private void CheckAddEmployeeAndGenerateEvaluation()
        {
            _LOG.Info("Check the start of semi annual period: started at " + DateTime.Now.ToString());
            IEnumerable<int> semiAnnualPeriods = _generationEvaluationService.CheckStartSemiAnnual();
            _LOG.Info("Result of check the start of semi annual period: " + string.Join(",", semiAnnualPeriods));

            if (semiAnnualPeriods != null && semiAnnualPeriods.Count() > 0)
            {
                AddAndGenerateSemiAnnual(semiAnnualPeriods);
            }
        }

        private void AddAndGenerateSemiAnnual(IEnumerable<int> semiAnnualPeriods)
        {
            foreach (int periodId in semiAnnualPeriods)
            {
                _LOG.Info("Auto add employee to semi annual period " + periodId + ": started at " + DateTime.Now.ToString());
                CommandResult addEmployeeResult = _generationEvaluationService.InsertEmployeeToSemiAnnualPeriod(periodId);
                _LOG.Info("Result of auto add employee to semi annual period " + periodId + ": " + (addEmployeeResult.IsSuccessful ? "0" : addEmployeeResult.Message));

                if (addEmployeeResult.IsSuccessful)
                {
                    _LOG.Info("Auto generate EPA to all employee of current semi annual period " + periodId + ": started at " + DateTime.Now.ToString());
                    CommandResult generateEvaluationResult = _generationEvaluationService.AutoGenerateEvaluation(periodId);
                    _LOG.Info("Result of auto generate EPA to all employee of current semi annual period " + periodId + ": " + (generateEvaluationResult.IsSuccessful ? "0" : generateEvaluationResult.Message));

                    if (generateEvaluationResult.IsSuccessful)
                    {
                        SendEmailSemiAnnual(periodId);
                    }
                }
            }
        }

        private void SendEmailSemiAnnual(int periodId)
        {
            throw new NotImplementedException();
        }
    }
}
