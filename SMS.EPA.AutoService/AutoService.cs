namespace SMS.EPA.AutoService
{
    using Core.Constants;
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
        private readonly IEvaluationService evaluationService;
        private readonly IPeriodService periodService;
        private readonly IPeriodMailService reminderMailService;
        private readonly IEmployeeEPAService employeeEPAService;
        private readonly IMonthlyService monthlyService;
        private readonly ISemiAnnualService semiAnnualService;
        private readonly IProbationService probationService;
        private readonly IPromotionService promotionService;
        private readonly ICryptographyService cryptographyService;
        private bool _isBusy;

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Reviewed")]
        public AutoService()
        {
            InitializeComponent();

            var schedulerTime = DateTime.Today.AddDays(1).AddHours(AppSetting.HoursToStartService);

            _updateCounterTimer = new Timer { Interval = schedulerTime.Subtract(DateTime.Now).TotalSeconds * 1000 };
            _updateCounterTimer.Elapsed += ExecuteJob;
            _updateCounterTimer.Enabled = false;

            cryptographyService = new CryptographyService();
            evaluationService = new EvaluationService();
            periodService = new PeriodService();
            reminderMailService = new PeriodMailService();
            employeeEPAService = new EmployeeEPAService();
            monthlyService = new MonthlyService();
            semiAnnualService = new SemiAnnualService();
            probationService = new ProbationService();
            promotionService = new PromotionService();
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
            if (AppSetting.IsRunAtStartService)
            {
                CheckAndExecute();
            }
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

#if DEBUG

        public void CheckAndExecuteForDebug()
        {
            CheckAndExecute();
        }

#endif

        private void CheckAndExecute()
        {
            if (AppSetting.EnableMonthlyPeriod)
            {
                monthlyService.CreateMonthlyPeriod();
                monthlyService.CheckMonthlyPeriod();
            }

            semiAnnualService.CheckSemiAnnualPeriod();
            probationService.CheckProbationPeriod();
            promotionService.CheckPromotionPeriod();

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
            IEnumerable<AutoEvaluationProcessModel> evaluationProcesses = evaluationService.CheckEndProcess();

            if (evaluationProcesses != null && evaluationProcesses.Any())
            {
                UpdateEndProcess(evaluationProcesses);
                UpdateTotalScoreOfSemiHasMonthly(evaluationProcesses.Where(x => x.ProcessId == (int)ProcessType.Evaluator && x.IsSemiHasMonthly));
                UpdateTotalScoreOfProcess(evaluationProcesses.Where(x => x.ProcessId >= (int)ProcessType.Reviewer));
            }
        }

        private void UpdateEndProcess(IEnumerable<AutoEvaluationProcessModel> evaluationProcesses)
        {
            var criteria = new UpdateIsEndProcessCriteria
            {
                EvaluationProcesses = evaluationProcesses
            };
            var commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = evaluationService.UpdateEndProcess(criteria);
            }

            string messageResult = commandResult.IsSuccessful ? "0" : commandResult.Message;
            Log.Info("Result of update status of processes that ended yesterday: " + messageResult);
        }

        private void UpdateTotalScoreOfSemiHasMonthly(IEnumerable<AutoEvaluationProcessModel> evaluationProcesses)
        {
            foreach (AutoEvaluationProcessModel evaluationProcess in evaluationProcesses)
            {
                EPAModel epaInfo = employeeEPAService.GetEPAInfoById(evaluationProcess.EPAId);
                var criteria = new UpdateTotalScoreOfSemiHasMonthlyCriteria
                {
                    EPAId = evaluationProcess.EPAId,
                    MonthlyEPA = epaInfo.MonthlyEPA
                };
                var commandResult = new CommandResult();

                if (criteria.IsValid())
                {
                    commandResult = evaluationService.UpdateTotalScoreOfSemiHasMonthly(criteria);
                }

                var messageResult = commandResult.IsSuccessful ? "0" : commandResult.Message;
                Log.Info("Result of update total score semi has monthly " + evaluationProcess.EPAId + " : " + messageResult);
            }
        }

        private void UpdateTotalScoreOfProcess(IEnumerable<AutoEvaluationProcessModel> evaluationProcesses)
        {
            foreach (AutoEvaluationProcessModel evaluationProcess in evaluationProcesses)
            {
                if (string.IsNullOrEmpty(evaluationProcess.TotalScore))
                {
                    EPAModel epaInfo = employeeEPAService.GetEPAInfoById(evaluationProcess.EPAId);

                    float? averageScore = GetAverageScorePreProcess(epaInfo, evaluationProcess.ProcessId);
                    Log.Info("Result of get average score of pre-process of EPA " + evaluationProcess.EPAId + " : " + averageScore);

                    var criteria = new UpdateTotalScoreOfProcessCriteria
                    {
                        EPAId = evaluationProcess.EPAId,
                        ProcessId = evaluationProcess.ProcessId,
                        AverageScore = averageScore.HasValue
                            ? cryptographyService.Encrypt(averageScore.Value.ToString(CultureInfo.InvariantCulture))
                            : string.Empty
                    };

                    var commandResult = new CommandResult();

                    if (criteria.IsValid())
                    {
                        commandResult = evaluationService.UpdateTotalScoreOfProcess(criteria);
                    }

                    var messageResult = commandResult.IsSuccessful ? "0" : commandResult.Message;
                    Log.Info("Result of update total score of process of EPA " + evaluationProcess.EPAId + " : " + messageResult);
                }
            }
        }

        private void CheckEndEvaluationPeriod()
        {
            var criteria = new GetEPAEndEvaluationPeriodCriteria();
            IEnumerable<EPAModel> allEvaluations = employeeEPAService.GetEPAEndEvaluationPeriod(criteria);

            if (allEvaluations != null && allEvaluations.Any())
            {
                UpdateFinalScoreAndSendMail(allEvaluations);
            }
        }

        private void UpdateFinalScoreAndSendMail(IEnumerable<EPAModel> epaInfos)
        {
            IList<int> periods = new List<int>();
            IEnumerable<StandardGradeModel> standardGrades = employeeEPAService.GetStandardGrade();

            foreach (EPAModel epaInfo in epaInfos)
            {
                float? performanceScore = GetFinalScoreOfEvalution(epaInfo);
                Log.Info("Result of get average performance score of EPA " + epaInfo.EPAId + " : " + performanceScore);

                epaInfo.Grade = performanceScore.HasValue
                    ? employeeEPAService.ConvertScoreToGrade(performanceScore, epaInfo.DisciplineScore, epaInfo.HasDisciplineScore, standardGrades)
                    : string.Empty;
                Log.Info("Result of get grade of final score of EPA " + epaInfo.EPAId + ": " + epaInfo.Grade);

                EncryptAndUpdateFinalScore(performanceScore, epaInfo.DisciplineScore, epaInfo.EPAId, epaInfo.Grade);

                if (AppSetting.SendMailEndPeriod)
                {
                    SendMailEndEvaluation(periods, epaInfo, performanceScore);
                }
            }

            SendMailEndPeriod(periods);
        }

        private void SendMailEndEvaluation(IList<int> periods, EPAModel epaInfo, float? averagePerformanceScore)
        {
            reminderMailService.SendEndEvaluationMailToSelf(epaInfo, averagePerformanceScore, epaInfo.DisciplineScore);

            if (epaInfo.Period != null)
            {
                if (epaInfo.IsProbation || epaInfo.IsPromotion)
                {
                    reminderMailService.SendEndEvaluationMailToHumanResourceByEmployee(epaInfo);
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
                    int lastestProcessOfPeriod = evaluationService.CheckLastestProcessOfPeriod(periodId);
                    Log.Info("Result of check the end of period " + periodId + " : " + lastestProcessOfPeriod);

                    if (lastestProcessOfPeriod == 1)
                    {
                        evaluationService.GetAndSendEmailToAllRelatedEmployee(
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
            IEnumerable<AutoEmployeePeriodModel> startedProcess = periodService.GetPeriodHasStartedProcess();

            foreach (AutoEmployeePeriodModel periodProcess in startedProcess)
            {
                evaluationService.GetAndSendEmailToAllRelatedEmployee(
                    periodProcess.PeriodId,
                    string.Empty,
                    periodProcess.ProcessId,
                    (int)MailMode.Start);
            }
        }

        private void CheckSendReminderEmail()
        {
            var criteria = new GetPeriodHasReminderProcessCriteria();
            IEnumerable<AutoEmployeePeriodModel> reminderProcess = evaluationService.GetPeriodHasReminderProcess(criteria);

            foreach (AutoEmployeePeriodModel periodProcess in reminderProcess)
            {
                evaluationService.GetAndSendEmailToAllRelatedEmployee(
                    periodProcess.PeriodId,
                    string.Empty,
                    periodProcess.ProcessId,
                    (int)MailMode.Reminder);
            }
        }

        private void EncryptAndUpdateFinalScore(
            float? performanceScore,
            float disciplineScore,
            int evaluationId,
            string grade)
        {
            float? finalScore = performanceScore + disciplineScore;
            finalScore = finalScore.HasValue ? (finalScore.Value > Constants.MaxFinalScore ? Constants.MaxFinalScore : finalScore.Value) : finalScore;

            string finalScoreEncrypt = finalScore.HasValue
                ? cryptographyService.Encrypt(finalScore.Value.ToString(CultureInfo.InvariantCulture))
                : string.Empty;
            Log.Info("Result of encrypt final score of EPA " + evaluationId + ": " + finalScoreEncrypt);

            string gradeEncrypt = cryptographyService.Encrypt(grade);
            Log.Info("Result of encrypt grade of EPA " + evaluationId + ": " + gradeEncrypt);

            var criteria = new UpdateFinalScoreEndEvaluationCriteria
            {
                EvaluationScores = UpdateFinalScoreEndEvaluationCriteria.ConvertToXml(
                    evaluationId,
                    finalScoreEncrypt,
                    disciplineScore,
                    gradeEncrypt)
            };

            CommandResult commandResult = evaluationService.UpdateFinalScoreEndEvaluation(criteria);
            string messageResult = commandResult.IsSuccessful ? "0" : commandResult.Message;
            Log.Info("Result of update final score and discipline score of EPA " + evaluationId + ": " + messageResult);
        }
    }
}