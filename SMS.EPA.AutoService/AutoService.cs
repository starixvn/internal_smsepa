namespace SMS.EPA.AutoService
{
    using Core.Enumerations;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using EPA.Criteria;
    using log4net;
    using Model;
    using Service;
    using Services.Implement;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.ServiceProcess;
    using System.Threading.Tasks;
    using System.Timers;

    public partial class AutoService : ServiceBase
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Timer _updateCounterTimer;
        private readonly IEvaluationService _evaluationService;
        private readonly IPeriodService _periodService;
        private readonly IAutoMailService _autoMailService;
        private readonly IEmployeeEPAService _employeeEPAService;
        private readonly IMonthlyService _monthlyService;
        private readonly ISemiAnnualService _semiAnnualService;
        private readonly IProbationService _probationService;
        private readonly IPromotionService _promotionService;
        private readonly ICryptographyService _cryptographyService;
        private bool _isBusy;

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Reviewed")]
        public AutoService()
        {
            InitializeComponent();

            var schedulerTime = DateTime.Today.AddDays(1).AddHours(AppSetting.HoursToStartService);

            _updateCounterTimer = new Timer { Interval = schedulerTime.Subtract(DateTime.Now).TotalSeconds * 1000 };
            _updateCounterTimer.Elapsed += ExecuteJob;
            _updateCounterTimer.Enabled = false;

            _cryptographyService = new CryptographyService();
            _evaluationService = new EvaluationService();
            _periodService = new PeriodService();
            _autoMailService = new AutoMailService();
            _employeeEPAService = new EmployeeEPAService();
            _monthlyService = new MonthlyService();
            _semiAnnualService = new SemiAnnualService();
            _probationService = new ProbationService();
            _promotionService = new PromotionService();
        }

        private static float? GetFinalScoreOfEvalution(EPAModel epaInfo)
        {
            float? finalScore = null;

            if (epaInfo.Process.Managements.Any() && epaInfo.Process.AverageScoreByManagements.HasValue)
            {
                finalScore = epaInfo.Process.AverageScoreByManagements.Value;
            }
            else
            {
                if (epaInfo.Process.Reviewers.Any() && epaInfo.Process.AverageScoreByReviewers.HasValue)
                {
                    finalScore = epaInfo.Process.AverageScoreByReviewers.Value;
                }
            }

            return finalScore;
        }

        private static float? GetAverageScorePreProcess(EPAModel epaInfo, int processId)
        {
            float? averageScore;

            switch (processId)
            {
                case (int)ProcessType.Reviewer:
                    averageScore = epaInfo.Process.AverageScoreByEvaluators;
                    break;
                case (int)ProcessType.Management:
                    averageScore = epaInfo.Process.AverageScoreByReviewers;
                    break;
                default:
                    averageScore = null;
                    break;
            }

            return averageScore;
        }

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

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification =
            "Reviewed. Suppression is OK here.")]
        private void ExecuteJob(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (Math.Abs(_updateCounterTimer.Interval - (AppSetting.TimerInterval * 1000)) > 0)
                {
                    _updateCounterTimer.Interval = AppSetting.TimerInterval * 1000;
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
                if (ex.InnerException != null)
                {
                    Log.Error("Error: " + ex.InnerException.Message);
                }
            }
        }

        private void CheckAndExecute()
        {
            if (AppSetting.EnableMonthlyPeriod)
            {
                _monthlyService.CreateMonthlyPeriod();
                _monthlyService.CheckMonthlyPeriod();
            }

            _semiAnnualService.CheckSemiAnnualPeriod();
            _probationService.CheckProbationPeriod();
            _promotionService.CheckPromotionPeriod();

            CheckSendMail();
            CheckEndProcess();
            CheckEndEvaluationPeriod();
        }

        private void CheckSendMail()
        {
            Task.Factory.StartNew(CheckSendNoticeAtBeginEmail);

            Task.Factory.StartNew(CheckSendReminderEmail);
        }

        private void CheckEndProcess()
        {
            IEnumerable<AutoEmployeePeriodModel> evaluationProcesses = _evaluationService.CheckEndProcess();

            if (evaluationProcesses != null && evaluationProcesses.Any())
            {
                UpdateEndProcess(evaluationProcesses);
                UpdateTotalScoreOfSemiHasMonthly(
                    evaluationProcesses.Where(x => x.ProcessId == (int)ProcessType.Evaluator && x.IsSemiHasMonthly));
                UpdateTotalScoreOfProcess(evaluationProcesses.Where(x => x.ProcessId >= (int)ProcessType.Reviewer));
            }
        }

        private void UpdateEndProcess(IEnumerable<AutoEmployeePeriodModel> evaluationProcesses)
        {
            var criteria = new UpdateIsEndProcessCriteria
            {
                EvaluationProcesses = evaluationProcesses
            };
            var commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = _evaluationService.UpdateEndProcess(criteria);
            }

            string messageResult = commandResult.IsSuccessful ? "0" : commandResult.Message;
            Log.Info("Result of update status of processes that ended yesterday: " + messageResult);
        }

        private void UpdateTotalScoreOfSemiHasMonthly(IEnumerable<AutoEmployeePeriodModel> evaluationProcesses)
        {
            foreach (AutoEmployeePeriodModel evaluationProcess in evaluationProcesses)
            {
                EPAModel epaInfo = _employeeEPAService.GetEPAInfoById(evaluationProcess.EPAId);
                var criteria = new UpdateTotalScoreOfSemiHasMonthlyCriteria
                {
                    EPAId = evaluationProcess.EPAId,
                    MonthlyEPA = epaInfo.MonthlyEPA
                };
                var commandResult = new CommandResult();

                if (criteria.IsValid())
                {
                    commandResult = _evaluationService.UpdateTotalScoreOfSemiHasMonthly(criteria);
                }

                var messageResult = commandResult.IsSuccessful ? "0" : commandResult.Message;
                Log.Info("Result of update total score semi has monthly " + evaluationProcess.EPAId + " : " +
                         messageResult);
            }
        }

        private void UpdateTotalScoreOfProcess(IEnumerable<AutoEmployeePeriodModel> evaluationProcesses)
        {
            foreach (AutoEmployeePeriodModel evaluationProcess in evaluationProcesses)
            {
                if (string.IsNullOrEmpty(evaluationProcess.TotalScore))
                {
                    EPAModel epaInfo = _employeeEPAService.GetEPAInfoById(evaluationProcess.EPAId);

                    float? averageScore = GetAverageScorePreProcess(epaInfo, evaluationProcess.ProcessId);
                    Log.Info("Result of get average score of pre-process of EPA " + evaluationProcess.EPAId + " : " +
                             averageScore);

                    var criteria = new UpdateTotalScoreOfProcessCriteria
                    {
                        EPAId = evaluationProcess.EPAId,
                        ProcessId = evaluationProcess.ProcessId,
                        AverageScore = averageScore.HasValue
                            ? _cryptographyService.Encrypt(averageScore.Value.ToString(CultureInfo.InvariantCulture))
                            : string.Empty
                    };

                    var commandResult = new CommandResult();

                    if (criteria.IsValid())
                    {
                        commandResult = _evaluationService.UpdateTotalScoreOfProcess(criteria);
                    }

                    var messageResult = commandResult.IsSuccessful ? "0" : commandResult.Message;
                    Log.Info("Result of update total score of process of EPA " + evaluationProcess.EPAId + " : " +
                             messageResult);
                }
            }
        }

        private void CheckEndEvaluationPeriod()
        {
            var criteria = new GetEPAEndEvaluationPeriodCriteria();
            IEnumerable<EPAModel> allEvaluations = _employeeEPAService.GetEPAEndEvaluationPeriod(criteria);

            if (allEvaluations != null && allEvaluations.Any())
            {
                UpdateFinalScoreAndSendMail(allEvaluations);
            }
        }

        private void UpdateFinalScoreAndSendMail(IEnumerable<EPAModel> epaInfos)
        {
            IList<int> periods = new List<int>();
            IEnumerable<StandardGradeModel> standardGrades = _employeeEPAService.GetStandardGrade();

            foreach (EPAModel epaInfo in epaInfos)
            {
                float? averagePerformanceScore = GetFinalScoreOfEvalution(epaInfo);
                Log.Info("Result of get average performance score of EPA " + epaInfo.EPAId + " : " +
                         averagePerformanceScore);

                float? finalScore = averagePerformanceScore.HasValue
                    ? (averagePerformanceScore.Value + epaInfo.DisciplineScore)
                    : (float?)null;

                epaInfo.Grade = finalScore.HasValue
                    ? _employeeEPAService.ConvertScoreToGrade(finalScore.Value, standardGrades)
                    : string.Empty;
                Log.Info("Result of get grade of final score of EPA " + epaInfo.EPAId + ": " + epaInfo.Grade);

                EncryptAndUpdateFinalScore(
                    averagePerformanceScore,
                    epaInfo.DisciplineScore,
                    epaInfo.EPAId,
                    epaInfo.Grade);

                if (AppSetting.SendMailEndPeriod)
                {
                    SendMailEndEvaluation(periods, epaInfo, averagePerformanceScore);
                }
            }

            SendMailEndPeriod(periods);
        }

        private void SendMailEndEvaluation(IList<int> periods, EPAModel epaInfo, float? averagePerformanceScore)
        {
            _autoMailService.SendEndEvaluationMailToSelf(
                epaInfo,
                averagePerformanceScore,
                epaInfo.DisciplineScore);

            if (epaInfo.Period != null)
            {
                if (epaInfo.IsProbation || epaInfo.IsPromotion)
                {
                    _autoMailService.SendEndEvaluationMailToHumanResourceByEmployee(epaInfo);
                }
                else
                {
                    if (epaInfo.IsSemiHasMonthly && !periods.Contains(epaInfo.Period.PeriodId))
                    {
                        periods.Add(epaInfo.Period.PeriodId);
                    }
                }
            }
        }

        private void SendMailEndPeriod(IList<int> periods)
        {
            if (AppSetting.SendMailEndPeriod && periods.Any())
            {
                foreach (int periodId in periods)
                {
                    int lastestProcessOfPeriod = _evaluationService.CheckLastestProcessOfPeriod(periodId);
                    Log.Info("Result of check the end of period " + periodId + " : " + lastestProcessOfPeriod);

                    if (lastestProcessOfPeriod == 1)
                    {
                        _evaluationService.GetAndSendEmailToAllRelatedEmployee(
                            periodId,
                            string.Empty,
                            (int)ProcessType.HumanResource,
                            (int)MailMode.EndPeriod);
                    }
                }
            }
        }

        private void CheckSendNoticeAtBeginEmail()
        {
            IEnumerable<AutoEmployeePeriodModel> startedProcess = _periodService.GetPeriodHasStartedProcess();

            foreach (AutoEmployeePeriodModel periodProcess in startedProcess)
            {
                _evaluationService.GetAndSendEmailToAllRelatedEmployee(
                    periodProcess.PeriodId,
                    string.Empty,
                    periodProcess.ProcessId,
                    (int)MailMode.Start);
            }
        }

        private void CheckSendReminderEmail()
        {
            var criteria = new GetPeriodHasReminderProcessCriteria();
            IEnumerable<AutoEmployeePeriodModel> reminderProcess =
                _evaluationService.GetPeriodHasReminderProcess(criteria);

            foreach (AutoEmployeePeriodModel periodProcess in reminderProcess)
            {
                if (!periodProcess.IsSemiHasMonthly
                    || (periodProcess.ProcessId != (int)ProcessType.Self
                        && periodProcess.ProcessId != (int)ProcessType.Evaluator))
                {
                    _evaluationService.GetAndSendEmailToAllRelatedEmployee(
                        periodProcess.PeriodId,
                        string.Empty,
                        periodProcess.ProcessId,
                        (int)MailMode.Reminder);
                }
            }
        }

        private void EncryptAndUpdateFinalScore(
            float? performanceScore,
            float disciplineScore,
            int evaluationId,
            string grade)
        {
            float? finalScore = performanceScore + disciplineScore;
            string finalScoreEncrypt = finalScore.HasValue
                ? _cryptographyService.Encrypt(finalScore.Value.ToString(CultureInfo.InvariantCulture))
                : string.Empty;
            Log.Info("Result of encrypt final score of EPA " + evaluationId + ": " + finalScoreEncrypt);

            string gradeEncrypt = _cryptographyService.Encrypt(grade);
            Log.Info("Result of encrypt grade of EPA " + evaluationId + ": " + gradeEncrypt);

            var criteria = new UpdateFinalScoreEndEvaluationCriteria
            {
                EvaluationScores = UpdateFinalScoreEndEvaluationCriteria.ConvertToXml(
                    evaluationId,
                    finalScoreEncrypt,
                    disciplineScore,
                    gradeEncrypt)
            };

            CommandResult commandResult = _evaluationService.UpdateFinalScoreEndEvaluation(criteria);
            string messageResult = commandResult.IsSuccessful ? "0" : commandResult.Message;
            Log.Info("Result of update final score and discipline score of EPA " + evaluationId + ": " + messageResult);
        }
    }
}