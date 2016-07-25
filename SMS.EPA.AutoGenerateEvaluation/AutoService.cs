namespace SMS.EPA.AutoGenerateEvaluation
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.ServiceProcess;
    using System.Threading.Tasks;
    using System.Timers;
    using log4net;
    using SMS.EPA.AutoGenerateEvaluation.Criteria;
    using SMS.EPA.AutoGenerateEvaluation.Criteria.Monthly;
    using SMS.EPA.AutoGenerateEvaluation.Service.Implement;
    using SMS.EPA.AutoGenerateEvaluation.Service.Interfaces;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Criteria.Period;
    using SMS.EPA.Criteria.Period.GenerationEvaluation;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.FlowManagement;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public partial class AutoService : ServiceBase
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Timer _updateCounterTimer;
        private readonly IEvaluationService _evaluationService;
        private readonly IPeriodService _periodService;
        private readonly IAutoMailService _autoMailService;
        private readonly IEmployeeEPAService _employeeEPAService;
        private bool _isBusy;

        public AutoService()
        {
            InitializeComponent();

            var schedulerTime = DateTime.Today.AddDays(1).AddHours(5);
            TimerInterval = int.Parse(ConfigurationManager.AppSettings["TimerInterval"]);

            _updateCounterTimer = new Timer { Interval = schedulerTime.Subtract(DateTime.Now).TotalSeconds * 1000 };
            _updateCounterTimer.Elapsed += ExecuteJob;
            _updateCounterTimer.Enabled = false;

            _evaluationService = new EvaluationService();
            _periodService = new PeriodService();
            _autoMailService = new AutoMailService();
            _employeeEPAService = new EmployeeEPAService();
        }

        public int TimerInterval { get; private set; }

        protected override void OnStart(string[] args)
        {
            Log.Info("Service started at " + DateTime.Now);
            _updateCounterTimer.Enabled = true;
        }

        protected override void OnStop()
        {
            _updateCounterTimer.Enabled = false;
            Log.Info("Service stopped at " + DateTime.Now);
        }

        private void ExecuteJob(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (Math.Abs(_updateCounterTimer.Interval - (TimerInterval * 1000)) > 0)
                {
                    _updateCounterTimer.Interval = TimerInterval * 1000;
                }

                if (!_isBusy)
                {
                    _isBusy = true;
                    CheckAndExecute();
                    _isBusy = false;
                }
            }
            catch (Exception ex)
            {
                _isBusy = false;
                Log.Error("Error: " + ex.InnerException.Message);
            }
        }

        private void CheckAndExecute()
        {
            if (ConfigurationManager.AppSettings["EnableMonthlyPeriod"].Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                CreateMonthlyPeriod();
                CheckMonthlyPeriod();
            }

            CheckSemiAnnualPeriod();
            CheckProbationPeriod();
            CheckPromotionPeriod();
            CheckSendMail();
            CheckEndProcess();
            CheckEndEvaluationPeriod();
        }

        #region Semi Annual

        private void CheckSemiAnnualPeriod()
        {
            Log.Info("Check the start of semi annual period: started at " + DateTime.Now);
            var criteria = new GetSemiAnnualPeriodStartedCriteria();
            IEnumerable<int> semiAnnualPeriods = _evaluationService.CheckStartSemiAnnual(criteria);
            Log.Info("Result of check the start of semi annual period: " + string.Join(",", semiAnnualPeriods));

            if (semiAnnualPeriods.Any())
            {
                AddEmployeeAndGenerateEPASemiAnnual(semiAnnualPeriods);
            }
        }

        private void AddEmployeeAndGenerateEPASemiAnnual(IEnumerable<int> semiAnnualPeriods)
        {
            bool sendMailAllWhenPeriodStarted = ConfigurationManager.AppSettings["SendMailAllWhenPeriodStarted"].Equals("true", StringComparison.OrdinalIgnoreCase);
            foreach (int periodId in semiAnnualPeriods)
            {
                Log.Info("Auto add employee to semi annual period " + periodId + ": started at " + DateTime.Now);
                var insertEmployeeCriteria = new InsertEmployeeToSemiAnnualPeriodCriteria
                {
                    PeriodId = periodId
                };
                CommandResult insertEmployeeResult = _evaluationService.InsertEmployeeToSemiAnnualPeriod(insertEmployeeCriteria);
                string messageResult = insertEmployeeResult.IsSuccessful ? "0" : insertEmployeeResult.Message;
                Log.Info("Result of auto add employee to semi annual period " + periodId + ": " + messageResult);

                Log.Info("Auto generate EPA to all employees of current semi annual period " + periodId + ": started at " + DateTime.Now);
                var generateEvaluationCriteria = new AutoGenerateEvaluationCriteria
                {
                    PeriodId = periodId
                };
                CommandResult generateEvaluationResult = _periodService.GenerateEvaluation(generateEvaluationCriteria);
                messageResult = generateEvaluationResult.IsSuccessful ? "0" : generateEvaluationResult.Message;
                Log.Info("Result of auto generate EPA to all employees of current semi annual period " + periodId + ": " + messageResult);

                if (sendMailAllWhenPeriodStarted)
                {
                    Task.Factory.StartNew(() =>
                    {
                        GetAndSendEmailToAllRelatedEmployee(periodId, string.Empty, null, (int)MailMode.Start);
                    });
                }
            }
        }

        #endregion Semi Annual

        #region Probation

        private void CheckProbationPeriod()
        {
            Log.Info("Check the start of probation evaluation: started at " + DateTime.Now);
            var criteria = new GetEmployeeProbationEvaluationCriteria();
            AutoEmployeePeriodModel employeePeriod = _evaluationService.GetEmployeeProbationEvaluation(criteria);
            Log.Info("Result of check the start of probation evaluation: employee: " + employeePeriod.EmployeeIds + "; periodId: " + employeePeriod.PeriodId);

            if (!string.IsNullOrWhiteSpace(employeePeriod.EmployeeIds) && employeePeriod.PeriodId > 0)
            {
                AddEmployeeAndGenerateEPAProbation(employeePeriod);
            }
        }

        private void AddEmployeeAndGenerateEPAProbation(AutoEmployeePeriodModel employeePeriod)
        {
            Log.Info("Auto add employee to probation period " + employeePeriod.PeriodId + ": started at " + DateTime.Now);
            var insertEmployeeCriteria = new InsertEmployeeToProbationPeriodCriteria
            {
                PeriodId = employeePeriod.PeriodId,
                EmployeeIds = employeePeriod.EmployeeIds
            };

            CommandResult insertEmployeeResult = _evaluationService.InsertEmployeeToProbationPeriod(insertEmployeeCriteria);
            string messageResult = insertEmployeeResult.IsSuccessful ? "0" : insertEmployeeResult.Message;
            Log.Info("Result of auto add employee to probation period " + employeePeriod.PeriodId + ": " + messageResult);

            Log.Info("Auto generate EPA to all employees of current probation period " + employeePeriod.PeriodId + ": started at " + DateTime.Now);
            var generateEvaluationCriteria = new AutoGenerateEvaluationCriteria
            {
                PeriodId = employeePeriod.PeriodId,
                EmployeeIds = employeePeriod.EmployeeIds
            };
            CommandResult generateEvaluationResult = _periodService.GenerateEvaluation(generateEvaluationCriteria);
            messageResult = generateEvaluationResult.IsSuccessful ? "0" : generateEvaluationResult.Message;
            Log.Info("Result of auto generate EPA to all employees of current probation period " + employeePeriod.PeriodId + ": " + messageResult);

            if (ConfigurationManager.AppSettings["SendMailAllWhenPeriodStarted"].Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                Task.Factory.StartNew(() =>
                {
                    GetAndSendEmailToAllRelatedEmployee(employeePeriod.PeriodId, employeePeriod.EmployeeIds, null, (int)MailMode.Start);
                });
            }
        }

        #endregion Probation

        #region Promotion

        private void CheckPromotionPeriod()
        {
            Log.Info("Check the start of promotion evaluation: started at " + DateTime.Now);
            var criteria = new GetEmployeePromotionEvaluationCriteria();
            IEnumerable<AutoEmployeePeriodModel> employeePeriods = _evaluationService.GetEmployeePromotionEvaluation(criteria);
            Log.Info("Result of check the start of promotion evaluation: ended at  " + DateTime.Now);

            if (employeePeriods != null && employeePeriods.Any())
            {
                GenerateEPAPromotion(employeePeriods);
            }
        }

        private void GenerateEPAPromotion(IEnumerable<AutoEmployeePeriodModel> employeePeriods)
        {
            bool sendMailAllWhenPeriodStarted = ConfigurationManager.AppSettings["SendMailAllWhenPeriodStarted"].Equals("true", StringComparison.OrdinalIgnoreCase);
            foreach (AutoEmployeePeriodModel period in employeePeriods)
            {
                Log.Info("Auto generate EPA to all employees of current promotion period " + period.PeriodId + ": started at " + DateTime.Now);
                var generateEvaluationCriteria = new AutoGenerateEvaluationCriteria
                {
                    PeriodId = period.PeriodId,
                    EmployeeIds = period.EmployeeIds
                };
                CommandResult generateEvaluationResult = _periodService.GenerateEvaluation(generateEvaluationCriteria);
                string messageResult = generateEvaluationResult.IsSuccessful ? "0" : generateEvaluationResult.Message;
                Log.Info("Result of auto generate EPA to all employees of current promotion period " + period.PeriodId + ": " + messageResult);

                if (sendMailAllWhenPeriodStarted)
                {
                    Task.Factory.StartNew(() =>
                    {
                        GetAndSendEmailToAllRelatedEmployee(period.PeriodId, period.EmployeeIds, null, (int)MailMode.Start);
                    });
                }
            }
        }

        #endregion Promotion

        #region Monthly

        private void CreateMonthlyPeriod()
        {
            var getEvaluationGlobalSettingByTargetTypeIdCriteria = new GetEvaluationGlobalSettingByTargetTypeIdCriteria()
            {
                TargetTypeId = (int)TargetType.Monthly
            };

            IEnumerable<FlowModel> processSettings = _periodService.GetEvaluationGlobalSettingByTargetTypeId(getEvaluationGlobalSettingByTargetTypeIdCriteria);

            if (processSettings != null && processSettings.Any())
            {
                DateTime today = DateTime.Today;
                int startDayCreateMonthlyPeriod = int.Parse(ConfigurationManager.AppSettings["StartDayCreateMonthlyPeriod"]);

                if (today.Day.Equals(startDayCreateMonthlyPeriod))
                {
                    var criteria = new AutoCreateMonthlyPeriodCriteria()
                    {
                        Name = today.AddMonths(1).ToString(Constants.FormatMonthlyPeriodName),
                        StartDate = today,
                        EndDate = today.AddDays(processSettings.Sum(p => p.Duration))
                    };

                    Log.Info("Create monthly period: started at " + DateTime.Now);
                    CommandResult commandResult = _periodService.AutoCreateMonthlyPeriod(criteria);
                    Log.Info("Result of Create monthly period: " + (commandResult.IsSuccessful ? "0" : commandResult.Message));
                }
            }
        }

        private void CheckMonthlyPeriod()
        {
            Log.Info("Check the start of monthly period: started at " + DateTime.Now);
            var criteria = new GetMonthlyPeriodStartedCriteria();
            IEnumerable<int> monthlyPeriods = _evaluationService.GetMonthlyPeriodStarted(criteria);
            Log.Info("Result of check the start of monthly period: " + string.Join(",", monthlyPeriods));

            if (monthlyPeriods.Any())
            {
                AddEmployeeAndGenerateEPAMonthly(monthlyPeriods);
            }
        }

        private void AddEmployeeAndGenerateEPAMonthly(IEnumerable<int> monthlyPeriods)
        {
            bool sendMailAllWhenPeriodStarted = ConfigurationManager.AppSettings["SendMailAllWhenPeriodStarted"].Equals("true", StringComparison.OrdinalIgnoreCase);
            foreach (int periodId in monthlyPeriods)
            {
                Log.Info("Auto add employee to monthly period " + periodId + ": started at " + DateTime.Now);
                var insertEmployeeCriteria = new InsertEmployeeToMonthlyPeriodCriteria
                {
                    PeriodId = periodId
                };
                CommandResult insertEmployeeResult = _evaluationService.InsertEmployeeToMonthlyPeriod(insertEmployeeCriteria);
                string messageResult = insertEmployeeResult.IsSuccessful ? "0" : insertEmployeeResult.Message;
                Log.Info("Result of auto add employee to monthly period " + periodId + ": " + messageResult);

                Log.Info("Auto generate EPA to all employees of current monthly period " + periodId + ": started at " + DateTime.Now);
                var generateEvaluationCriteria = new AutoGenerateEvaluationCriteria
                {
                    PeriodId = periodId
                };
                CommandResult generateEvaluationResult = _periodService.GenerateEvaluation(generateEvaluationCriteria);
                messageResult = generateEvaluationResult.IsSuccessful ? "0" : generateEvaluationResult.Message;
                Log.Info("Result of auto generate EPA to all employees of current monthly period " + periodId + ": " + messageResult);

                if (sendMailAllWhenPeriodStarted)
                {
                    Task.Factory.StartNew(() =>
                    {
                        GetAndSendEmailToAllRelatedEmployee(periodId, string.Empty, null, (int)MailMode.Start);
                    });
                }
            }
        }

        #endregion Monthly

        private void CheckSendMail()
        {
            Task.Factory.StartNew(CheckSendNoticeAtBeginEmail);

            Task.Factory.StartNew(CheckSendReminderEmail);
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private void CheckEndProcess()
        {
            Log.Info("Check process that ended yesterday: started at " + DateTime.Now);
            var checkEndProcessCriteria = new CheckEndProcessCriteria();
            IEnumerable<AutoEmployeePeriodModel> evaluationProcesses = _evaluationService.CheckEndProcess(checkEndProcessCriteria);
            Log.Info("Check process that ended yesterday: ended at " + DateTime.Now);

            if (evaluationProcesses != null && evaluationProcesses.Any())
            {
                var updateEndProcessCriteria = new UpdateIsEndProcessCriteria()
                {
                    EvaluationProcesses = evaluationProcesses
                };

                Log.Info("Update status of processes that ended yesterday: started at " + DateTime.Now);
                CommandResult updateIsEndProcess = _evaluationService.UpdateEndProcess(updateEndProcessCriteria);
                string messageResult = updateIsEndProcess.IsSuccessful ? "0" : updateIsEndProcess.Message;
                Log.Info("Result of update status of processes that ended yesterday: " + messageResult);

                IList<AutoEmployeePeriodModel> semiEvaluationProcesses = evaluationProcesses
                    .Where(evaluationProcess => evaluationProcess.ProcessId == (int)ProcessType.Evaluator
                        && evaluationProcess.TargetTypeId == (int)TargetType.SemiAnnual
                        && evaluationProcess.HasMonthlyEPA).ToList();

                foreach (AutoEmployeePeriodModel evaluationProcess in semiEvaluationProcesses)
                {
                    var getEPAInfoCriteria = new GetEPAInfoByIdCriteria
                    {
                        EPAId = evaluationProcess.EPAId
                    };
                    EPAModel epaInfo = _employeeEPAService.GetEPAInfoById(getEPAInfoCriteria);

                    var updateTotalScoreOfSemiHasMonthlyCriteria = new UpdateTotalScoreOfSemiHasMonthlyCriteria
                    {
                        EPAId = evaluationProcess.EPAId,
                        MonthlyEPA = epaInfo.MonthlyEPA
                    };
                    Log.Info("Update total score of semi has monthly " + evaluationProcess.EPAId + " : started at " + DateTime.Now);
                    CommandResult commandResult = _evaluationService.UpdateTotalScoreOfSemiHasMonthly(updateTotalScoreOfSemiHasMonthlyCriteria);
                    messageResult = commandResult.IsSuccessful ? "0" : commandResult.Message;
                    Log.Info("Result of update total score semi has monthly " + evaluationProcess.EPAId + " : " + messageResult);
                }

                foreach (AutoEmployeePeriodModel evaluationProcess in evaluationProcesses.Where(evaluationProcess => evaluationProcess.ProcessId >= (int)ProcessType.Reviewer))
                {
                    if (string.IsNullOrEmpty(evaluationProcess.TotalScore))
                    {
                        Log.Info("Get information of EPA " + evaluationProcess.EPAId + " : started at " + DateTime.Now);
                        var getEvaluationInfoCriteria = new GetEPAInfoByIdCriteria
                        {
                            EPAId = evaluationProcess.EPAId
                        };
                        EPAModel evaluationInfo = _employeeEPAService.GetEPAInfoById(getEvaluationInfoCriteria);
                        Log.Info("Get information of EPA " + evaluationProcess.EPAId + " : ended at " + DateTime.Now);

                        Log.Info("Get average score of pre-process of EPA " + evaluationProcess.EPAId + " : started at " + DateTime.Now);
                        float averageScore = GetAverageScorePreProcess(evaluationInfo, evaluationProcess.ProcessId);
                        Log.Info("Result of get average score of pre-process of EPA " + evaluationProcess.EPAId + " : " + averageScore);

                        var updateTotalScoreCriteria = new UpdateTotalScoreOfProcessCriteria
                        {
                            EPAId = evaluationProcess.EPAId,
                            ProcessId = evaluationProcess.ProcessId,
                            AverageScore = EncodeService.Encode(averageScore.ToString())
                        };
                        Log.Info("Update total score of process of EPA " + evaluationProcess.EPAId + " : started at " + DateTime.Now);
                        CommandResult commandResult = _evaluationService.UpdateTotalScoreOfProcess(updateTotalScoreCriteria);
                        messageResult = commandResult.IsSuccessful ? "0" : commandResult.Message;
                        Log.Info("Result of update total score of process of EPA " + evaluationProcess.EPAId + " : " + messageResult);
                    }
                }
            }
        }

        private void CheckEndEvaluationPeriod()
        {
            Log.Info("Get all EPA that period end date is today or ended yesterday or close but not have final score: started at " + DateTime.Now);
            var getAllEndedEvaluationCriteria = new GetEPAEndEvaluationPeriodCriteria();
            IEnumerable<EPAModel> allEvaluations = _employeeEPAService.GetEPAEndEvaluationPeriod(getAllEndedEvaluationCriteria);
            Log.Info("Get all EPA that period end date is today or ended yesterday or close but not have final score: ended at " + DateTime.Now);

            if (allEvaluations != null && allEvaluations.Any())
            {
                UpdateFinalScoreAndSendMail(allEvaluations);
            }
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private void UpdateFinalScoreAndSendMail(IEnumerable<EPAModel> evaluationInfos)
        {
            IList<int> periods = new List<int>();
            bool hasSendMailEndEvaluation = ConfigurationManager.AppSettings["SendMailEndPeriod"] != null
                && ConfigurationManager.AppSettings["SendMailEndPeriod"].Equals("true", StringComparison.OrdinalIgnoreCase);

            var criteria = new GetStandardGradeCriteria();
            IEnumerable<StandardGradeModel> standardGrades = _employeeEPAService.GetStandardGrade(criteria);

            foreach (EPAModel evaluationInfo in evaluationInfos)
            {
                Log.Info("Get average performance score of EPA " + evaluationInfo.EPAId + " : started at " + DateTime.Now);
                float averagePerformanceScore = GetFinalScoreOfEvalution(evaluationInfo);
                Log.Info("Result of get average performance score of EPA " + evaluationInfo.EPAId + " :  " + averagePerformanceScore);

                float finalScore = averagePerformanceScore + evaluationInfo.DisciplineScore;
                finalScore = finalScore >= 0 ? finalScore : 0;

                Log.Info("Get grade of final score of EPA " + evaluationInfo.EPAId + ": started at: " + DateTime.Now);
                evaluationInfo.Grade = _employeeEPAService.ConvertScoreToGrade(finalScore, standardGrades);
                Log.Info("Result of get grade of final score of EPA " + evaluationInfo.EPAId + ": " + evaluationInfo.Grade);

                EncodeAndUpdateFinalScore(averagePerformanceScore, evaluationInfo.DisciplineScore, evaluationInfo.EPAId, evaluationInfo.Grade);

                if (hasSendMailEndEvaluation)
                {
                    SendEndEvaluationMailToSelf(evaluationInfo, averagePerformanceScore, evaluationInfo.DisciplineScore);

                    if (evaluationInfo.Period != null)
                    {
                        if (evaluationInfo.Period.TargetTypeId == (int)TargetType.Probation
                            || evaluationInfo.Period.TargetTypeId == (int)TargetType.Promotion)
                        {
                            Log.Info("Send end email to human resource of probation or promotion period " + evaluationInfo.Period.PeriodId + " : started at " + DateTime.Now);
                            _autoMailService.SendEndEvaluationMailToHumanResourceByEmployee(evaluationInfo);
                        }
                        else if ((evaluationInfo.Period.TargetTypeId == (int)TargetType.SemiAnnual || evaluationInfo.Period.TargetTypeId == (int)TargetType.Monthly)
                            && !periods.Contains(evaluationInfo.Period.PeriodId))
                        {
                            periods.Add(evaluationInfo.Period.PeriodId);
                        }
                    }
                }
            }

            if (hasSendMailEndEvaluation && periods.Any())
            {
                foreach (int periodId in periods)
                {
                    Log.Info("Check the end of period " + periodId + " : started at " + DateTime.Now);
                    var checkLastestProcessCriteria = new CheckLastestProcessOfPeriodCriteria
                    {
                        PeriodId = periodId
                    };
                    int lastestProcessOfPeriod = _evaluationService.CheckLastestProcessOfPeriod(checkLastestProcessCriteria);
                    Log.Info("Result of check the end of period " + periodId + " : " + lastestProcessOfPeriod);

                    if (lastestProcessOfPeriod == 1)
                    {
                        GetAndSendEmailToAllRelatedEmployee(periodId, string.Empty, (int)ProcessType.HumanResource, (int)MailMode.EndPeriod);
                    }
                }
            }
        }

        private void SendEndEvaluationMailToSelf(EPAModel evaluationInfo, float averagePerformanceScore, float disciplineScore)
        {
            Log.Info("Send email to employee " + evaluationInfo.Employee.NickName + " that ended evaluation : started at " + DateTime.Now);
            _autoMailService.SendEndEvaluationMailToSelf(evaluationInfo, averagePerformanceScore, disciplineScore);
        }

        private void CheckSendNoticeAtBeginEmail()
        {
            Log.Info("Get period has process evaluation started today: started at " + DateTime.Now);
            var criteria = new GetPeriodHasStartedProcessCriteria();
            IEnumerable<AutoEmployeePeriodModel> startedProcess = _periodService.GetPeriodHasStartedProcess(criteria);
            Log.Info("Result of get period has process evaluation started today: ended at " + DateTime.Now);

            foreach (AutoEmployeePeriodModel periodProcess in startedProcess)
            {
                GetAndSendEmailToAllRelatedEmployee(periodProcess.PeriodId, string.Empty, periodProcess.ProcessId, (int)MailMode.Start);
            }
        }

        private void CheckSendReminderEmail()
        {
            Log.Info("Get period has process evaluation reminder today: started at " + DateTime.Now);
            var criteria = new GetPeriodHasReminderProcessCriteria();
            IEnumerable<AutoEmployeePeriodModel> reminderProcess = _evaluationService.GetPeriodHasReminderProcess(criteria);
            Log.Info("Result of get period has process evaluation reminder today: ended at " + DateTime.Now);

            foreach (AutoEmployeePeriodModel periodProcess in reminderProcess)
            {
                if (periodProcess.TargetTypeId == (int)TargetType.SemiAnnual
                    && (periodProcess.ProcessId == (int)ProcessType.Self || periodProcess.ProcessId == (int)ProcessType.Evaluator)
                    && periodProcess.HasMonthlyEPA)
                {
                    continue;
                }
                else
                {
                    GetAndSendEmailToAllRelatedEmployee(periodProcess.PeriodId, string.Empty, periodProcess.ProcessId, (int)MailMode.Reminder);
                }
            }
        }

        private void GetAndSendEmailToAllRelatedEmployee(
            int periodId,
            string employeeIds,
            int? processId,
            int mailMode)
        {
            Log.Info("Get period name of periodId " + periodId + ": started at " + DateTime.Now);
            PeriodModel period = GetPeriodInformation(periodId);
            Log.Info("Result of get period name of periodId " + periodId + ": " + period.Name);

            Log.Info("Get employee setting of periodId " + periodId + ": started at " + DateTime.Now);
            IEnumerable<PeriodFlowModel> employeeSettings = GetEmployeeSettings(periodId, employeeIds, processId);
            Log.Info("Get employee setting of periodId " + periodId + ": ended at " + DateTime.Now);

            switch (mailMode)
            {
                case (int)MailMode.Start:
                    Log.Info("Send start email to all related employee of periodId " + periodId + ": started at " + DateTime.Now);
                    _autoMailService.SendStartMail(period, employeeSettings);
                    break;

                case (int)MailMode.Reminder:
                    Log.Info("Send reminder email to all related employee of periodId " + periodId + ": started at " + DateTime.Now);
                    _autoMailService.SendReminderMail(period, employeeSettings);
                    break;

                case (int)MailMode.EndPeriod:
                    Log.Info("Send end email to all related employee of periodId " + periodId + ": started at " + DateTime.Now);
                    _autoMailService.SendEndEvaluationMailToHumanResourceByPeriod(period, employeeSettings);
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

        private float GetFinalScoreOfEvalution(EPAModel evaluationInfo)
        {
            float finalScore = 0;

            if (evaluationInfo.Process.Managements.Any())
            {
                finalScore = evaluationInfo.Process.AverageScoreByManagements;
            }
            else if (evaluationInfo.Process.Reviewers.Any())
            {
                finalScore = evaluationInfo.Process.AverageScoreByReviewers;
            }
            else if (evaluationInfo.Process.Evaluators.Any())
            {
                finalScore = evaluationInfo.Process.AverageScoreByEvaluators;
            }

            return finalScore;
        }

        private void EncodeAndUpdateFinalScore(float performanceScore, float disciplineScore, int evaluationId, string grade)
        {
            float finalScore = performanceScore + disciplineScore;
            Log.Info("Encode final score of EPA " + evaluationId + ": started at: " + DateTime.Now);
            string finalScoreEncode = EncodeService.Encode(finalScore.ToString());
            Log.Info("Result of encode final score of EPA " + evaluationId + ": " + finalScoreEncode);

            Log.Info("Encode grade of EPA " + evaluationId + ": started at: " + DateTime.Now);
            string gradeEncode = EncodeService.Encode(grade);
            Log.Info("Result of encode grade of EPA " + evaluationId + ": " + gradeEncode);

            var criteria = new UpdateFinalScoreEndEvaluationCriteria();
            criteria.EvaluationScores = criteria.ConvertToXml(evaluationId, finalScoreEncode, disciplineScore, gradeEncode);

            Log.Info("Update final score and discipline score of EPA " + evaluationId + ": started at: " + DateTime.Now);
            CommandResult commandResult = _evaluationService.UpdateFinalScoreEndEvaluation(criteria);
            Log.Info("Result of update final score and discipline score of EPA " + evaluationId + ": " + (commandResult.IsSuccessful ? "0" : commandResult.Message));
        }

        private float GetAverageScorePreProcess(EPAModel evaluationInfo, int processId)
        {
            float averageScore = 0;

            switch (processId)
            {
                case (int)ProcessType.Reviewer:
                    averageScore = evaluationInfo.Process.AverageScoreByEvaluators;
                    break;

                case (int)ProcessType.Management:
                    averageScore = evaluationInfo.Process.Reviewers.Any() ? evaluationInfo.Process.AverageScoreByReviewers : evaluationInfo.Process.AverageScoreByEvaluators;
                    break;
            }

            return averageScore;
        }
    }
}