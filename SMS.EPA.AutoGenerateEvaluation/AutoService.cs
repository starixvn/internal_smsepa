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
    using SMS.EPA.AutoGenerateEvaluation.Criteria;
    using SMS.EPA.AutoGenerateEvaluation.Service.Implement;
    using SMS.EPA.AutoGenerateEvaluation.Service.Interfaces;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.Period;
    using SMS.EPA.Criteria.Period.GenerationEvaluation;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public partial class AutoService : ServiceBase
    {
        private static readonly ILog _LOG = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool _isBusy = false;
        private Timer _updateCounterTimer = null;
        private DateTime _schedulerTime;
        private IGenerationEvaluationService _generationEvaluationService;
        private IPeriodService _periodService;
        private IAutoMailService _autoMailService;

        public AutoService()
        {
            InitializeComponent();

            _schedulerTime = DateTime.Today.AddDays(1).AddHours(0);
            TimerInterval = int.Parse(ConfigurationManager.AppSettings["TimerInterval"]);

            _updateCounterTimer = new Timer();
            _updateCounterTimer.Interval = _schedulerTime.Subtract(DateTime.Now).TotalSeconds * 1000;
            _updateCounterTimer.Elapsed += new ElapsedEventHandler(ExecuteJob);
            _updateCounterTimer.Enabled = false;

            _generationEvaluationService = new GenerationEvaluationService();
            _periodService = new PeriodService();
            _autoMailService = new AutoMailService();
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
                    CheckAndExecute();
                    _isBusy = false;
                }

                if (_updateCounterTimer.Interval != TimerInterval)
                {
                    _updateCounterTimer.Interval = TimerInterval;
                }
            }
            catch (Exception ex)
            {
                _isBusy = false;
                _LOG.Error("Error: " + ex.InnerException.Message);
            }
        }

        private void CheckAndExecute()
        {
            CheckSemiAnnualPeriod();
            CheckProbationPeriod();
            CheckPromotionPeriod();
            CheckSendMail();
        }

        private void CheckSemiAnnualPeriod()
        {
            _LOG.Info("Check the start of semi annual period: started at " + DateTime.Now.ToString());
            var criteria = new GetSemiAnnualPeriodStartedCriteria();
            IEnumerable<int> semiAnnualPeriods = _generationEvaluationService.CheckStartSemiAnnual(criteria);
            _LOG.Info("Result of check the start of semi annual period: " + string.Join(",", semiAnnualPeriods));

            if (semiAnnualPeriods != null && semiAnnualPeriods.Count() > 0)
            {
                AddEmployeeAndGenerateEPASemiAnnual(semiAnnualPeriods);
            }
        }

        private void CheckProbationPeriod()
        {
            _LOG.Info("Check the start of probation evaluation: started at " + DateTime.Now.ToString());
            var criteria = new GetEmployeeProbationEvaluationCriteria();
            AutoEmployeePeriodModel employeePeriod = _generationEvaluationService.GetEmployeeProbationEvaluation(criteria);
            _LOG.Info("Result of check the start of probation evaluation: employee: " + employeePeriod.EmployeeIds + "; periodId: " + employeePeriod.PeriodId);

            if (!string.IsNullOrWhiteSpace(employeePeriod.EmployeeIds) && employeePeriod.PeriodId > 0)
            {
                AddEmployeeAndGenerateEPAProbation(employeePeriod);
            }
        }

        private void CheckPromotionPeriod()
        {
            _LOG.Info("Check the start of promotion evaluation: started at " + DateTime.Now.ToString());
            var criteria = new GetEmployeePromotionEvaluationCriteria();
            IEnumerable<AutoEmployeePeriodModel> employeePeriods = _generationEvaluationService.GetEmployeePromotionEvaluation(criteria);
            _LOG.Info("Result of check the start of promotion evaluation: ended at  " + DateTime.Now.ToString());

            if (employeePeriods != null && employeePeriods.Count() > 0)
            {
                GenerateEPAPromotion(employeePeriods);
            }
        }

        private void CheckSendMail()
        {
            CheckSendNoticeAtBeginEmail();
            CheckSendReminderEmail();
        }

        private void AddEmployeeAndGenerateEPASemiAnnual(IEnumerable<int> semiAnnualPeriods)
        {
            foreach (int periodId in semiAnnualPeriods)
            {
                _LOG.Info("Auto add employee to semi annual period " + periodId + ": started at " + DateTime.Now.ToString());
                var insertEmployeeCriteria = new InsertEmployeeToSemiAnnualPeriodCriteria
                {
                    PeriodId = periodId
                };
                CommandResult insertEmployeeResult = _generationEvaluationService.InsertEmployeeToSemiAnnualPeriod(insertEmployeeCriteria);
                _LOG.Info("Result of auto add employee to semi annual period " + periodId + ": " + (insertEmployeeResult.IsSuccessful ? "0" : insertEmployeeResult.Message));

                if (insertEmployeeResult.IsSuccessful)
                {
                    _LOG.Info("Auto generate EPA to all employees of current semi annual period " + periodId + ": started at " + DateTime.Now.ToString());
                    var generateEvaluationCriteria = new AutoGenerateEvaluationCriteria
                    {
                        PeriodId = periodId
                    };
                    CommandResult generateEvaluationResult = _periodService.GenerateEvaluation(generateEvaluationCriteria);
                    _LOG.Info("Result of auto generate EPA to all employees of current semi annual period " + periodId + ": " + (generateEvaluationResult.IsSuccessful ? "0" : generateEvaluationResult.Message));

                    _LOG.Info("Check period has send mail to all employees when period started: started at " + DateTime.Now.ToString());
                    var checkSendMailAllCriteria = new CheckPeriodStartedSendMailAllCriteria();
                    bool hasSendMailAll = _generationEvaluationService.CheckPeriodStartedSendMailAll(checkSendMailAllCriteria);
                    _LOG.Info("Result of check period has send mail to all employees when period started: " + hasSendMailAll);

                    if (generateEvaluationResult.IsSuccessful && hasSendMailAll)
                    {
                        GetAndSendEmailToAllRelatedEmployee(periodId, string.Empty, null, (int)MailMode.Start);
                    }
                }
            }
        }

        private void AddEmployeeAndGenerateEPAProbation(AutoEmployeePeriodModel employeePeriod)
        {
            _LOG.Info("Auto add employee to probation period " + employeePeriod.PeriodId + ": started at " + DateTime.Now.ToString());
            var insertEmployeeCriteria = new InsertEmployeeToProbationPeriodCriteria
            {
                PeriodId = employeePeriod.PeriodId,
                EmployeeIds = employeePeriod.EmployeeIds
            };

            CommandResult insertEmployeeResult = _generationEvaluationService.InsertEmployeeToProbationPeriod(insertEmployeeCriteria);
            _LOG.Info("Result of auto add employee to probation period " + employeePeriod.PeriodId + ": " + (insertEmployeeResult.IsSuccessful ? "0" : insertEmployeeResult.Message));

            if (insertEmployeeResult.IsSuccessful)
            {
                _LOG.Info("Auto generate EPA to all employees of current probation period " + employeePeriod.PeriodId + ": started at " + DateTime.Now.ToString());
                var generateEvaluationCriteria = new AutoGenerateEvaluationCriteria
                {
                    PeriodId = employeePeriod.PeriodId,
                    EmployeeIds = employeePeriod.EmployeeIds
                };
                CommandResult generateEvaluationResult = _periodService.GenerateEvaluation(generateEvaluationCriteria);
                _LOG.Info("Result of auto generate EPA to all employees of current probation period " + employeePeriod.PeriodId + ": " + (generateEvaluationResult.IsSuccessful ? "0" : generateEvaluationResult.Message));

                _LOG.Info("Check period has send mail to all employees when period started: started at " + DateTime.Now.ToString());
                var checkSendMailAllCriteria = new CheckPeriodStartedSendMailAllCriteria();
                bool hasSendMailAll = _generationEvaluationService.CheckPeriodStartedSendMailAll(checkSendMailAllCriteria);
                _LOG.Info("Result of check period has send mail to all employees when period started: " + hasSendMailAll);

                if (generateEvaluationResult.IsSuccessful && hasSendMailAll)
                {
                    GetAndSendEmailToAllRelatedEmployee(employeePeriod.PeriodId, employeePeriod.EmployeeIds, null, (int)MailMode.Start);
                }
            }
        }

        private void GenerateEPAPromotion(IEnumerable<AutoEmployeePeriodModel> employeePeriods)
        {
            _LOG.Info("Check period has send mail to all employees when period started: started at " + DateTime.Now.ToString());
            var checkSendMailAllCriteria = new CheckPeriodStartedSendMailAllCriteria();
            bool hasSendMailAll = _generationEvaluationService.CheckPeriodStartedSendMailAll(checkSendMailAllCriteria);
            _LOG.Info("Result of check period has send mail to all employees when period started: " + hasSendMailAll);

            foreach (AutoEmployeePeriodModel period in employeePeriods)
            {
                _LOG.Info("Auto generate EPA to all employees of current promotion period " + period.PeriodId + ": started at " + DateTime.Now.ToString());
                var generateEvaluationCriteria = new AutoGenerateEvaluationCriteria
                {
                    PeriodId = period.PeriodId,
                    EmployeeIds = period.EmployeeIds
                };
                CommandResult generateEvaluationResult = _periodService.GenerateEvaluation(generateEvaluationCriteria);
                _LOG.Info("Result of auto generate EPA to all employees of current promotion period " + period.PeriodId + ": " + (generateEvaluationResult.IsSuccessful ? "0" : generateEvaluationResult.Message));

                if (generateEvaluationResult.IsSuccessful && hasSendMailAll)
                {
                    GetAndSendEmailToAllRelatedEmployee(period.PeriodId, period.EmployeeIds, null, (int)MailMode.Start);
                }
            }
        }

        private void CheckSendNoticeAtBeginEmail()
        {
            _LOG.Info("Get period has process evaluation started today: started at " + DateTime.Now.ToString());
            var criteria = new GetPeriodHasStartedProcessCriteria();
            IEnumerable<AutoEmployeePeriodModel> startedProcess = _periodService.GetPeriodHasStartedProcess(criteria);
            _LOG.Info("Result of get period has process evaluation started today: ended at " + DateTime.Now.ToString());

            foreach (AutoEmployeePeriodModel periodProcess in startedProcess)
            {
                GetAndSendEmailToAllRelatedEmployee(periodProcess.PeriodId, string.Empty, periodProcess.ProcessId, (int)MailMode.Start);
            }
        }

        private void CheckSendReminderEmail()
        {
            _LOG.Info("Get period has process evaluation reminder today: started at " + DateTime.Now.ToString());
            var criteria = new GetPeriodHasReminderProcessCriteria();
            IEnumerable<AutoEmployeePeriodModel> reminderProcess = _generationEvaluationService.GetPeriodHasReminderProcess(criteria);
            _LOG.Info("Result of get period has process evaluation reminder today: ended at " + DateTime.Now.ToString());

            foreach (AutoEmployeePeriodModel periodProcess in reminderProcess)
            {
                GetAndSendEmailToAllRelatedEmployee(periodProcess.PeriodId, string.Empty, periodProcess.ProcessId, (int)MailMode.Reminder);
            }
        }

        private void GetAndSendEmailToAllRelatedEmployee(
            int periodId,
            string employeeIds,
            int? processId,
            int mailMode)
        {
            _LOG.Info("Get period name of periodId " + periodId + ": started at " + DateTime.Now.ToString());
            PeriodModel period = GetPeriodInformation(periodId);
            _LOG.Info("Result of get period name of periodId " + periodId + ": " + period.Name);

            _LOG.Info("Get employee setting of periodId " + periodId + ": started at " + DateTime.Now.ToString());
            IEnumerable<PeriodFlowModel> employeeSettings = GetEmployeeSettings(periodId, employeeIds, processId);
            _LOG.Info("Get employee setting of periodId " + periodId + ": ended at " + DateTime.Now.ToString());

            switch (mailMode)
            {
                case (int)MailMode.Start:
                    _LOG.Info("Send start email to all related employee of periodId " + periodId + ": started at " + DateTime.Now.ToString());
                    _autoMailService.SendStartMail(period, employeeSettings);
                    break;
                case (int)MailMode.Reminder:
                    _LOG.Info("Send reminder email to all related employee of periodId " + periodId + ": started at " + DateTime.Now.ToString());
                    _autoMailService.SendReminderMail(period, employeeSettings);
                    break;
            }
        }

        private PeriodModel GetPeriodInformation(int periodId)
        {
            var period = new PeriodModel();

            var criteria = new GetPeriodByIdCriteria
            {
                PeriodId = periodId
            };
             period = _periodService.GetPeriodById(criteria);

             return period;
        }

        private IEnumerable<PeriodFlowModel> GetEmployeeSettings(int periodId, string employeeIds, int? processId)
        {
            IEnumerable<PeriodFlowModel> employeeSettings = new List<PeriodFlowModel>();

            var getEmployeeSettingCriteria = new GetEmployeeSettingByPeriodIdCriteria
            {
                PeriodId = periodId,
                EmployeeIds = employeeIds,
                ProcessId = processId
            };
            employeeSettings = _periodService.GetEmployeeSettingByPeriodId(getEmployeeSettingCriteria);

            return employeeSettings;
        }
    }
}
