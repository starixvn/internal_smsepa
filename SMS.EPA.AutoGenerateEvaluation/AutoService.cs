namespace SMS.EPA.AutoGenerateEvaluation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.ServiceProcess;
    using System.Threading.Tasks;
    using System.Timers;
    using log4net;
    using SMS.EPA.AutoGenerateEvaluation.Criteria;
    using SMS.EPA.AutoGenerateEvaluation.Service;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Criteria.Period.GenerationEvaluation;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public partial class AutoService : ServiceBase
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Timer updateCounterTimer;
        private readonly IEvaluationService evaluationService;
        private readonly IPeriodService periodService;
        private readonly IAutoMailService autoMailService;
        private readonly IEmployeeEPAService employeeEPAService;
        private readonly IMonthlyService monthlyService;
        private readonly ISemiAnnualService semiAnnualService;
        private readonly IProbationService probationService;
        private readonly IPromotionService promotionService;
        private bool _isBusy;

        public AutoService()
        {
            InitializeComponent();

            var schedulerTime = DateTime.Today.AddDays(1).AddHours(5);

            updateCounterTimer = new Timer { Interval = schedulerTime.Subtract(DateTime.Now).TotalSeconds * 1000 };
            updateCounterTimer.Elapsed += ExecuteJob;
            updateCounterTimer.Enabled = false;

            evaluationService = new EvaluationService();
            periodService = new PeriodService();
            autoMailService = new AutoMailService();
            employeeEPAService = new EmployeeEPAService();
            monthlyService = new MonthlyService();
            semiAnnualService = new SemiAnnualService();
            probationService = new ProbationService();
            promotionService = new PromotionService();
        }

        protected override void OnStart(string[] args)
        {
            Log.Info("Service started at " + DateTime.Now);
            updateCounterTimer.Enabled = true;
        }

        protected override void OnStop()
        {
            updateCounterTimer.Enabled = false;
            Log.Info("Service stopped at " + DateTime.Now);
        }

        private void ExecuteJob(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (Math.Abs(updateCounterTimer.Interval - (AppSetting.TimerInterval * 1000)) > 0)
                {
                    updateCounterTimer.Interval = AppSetting.TimerInterval * 1000;
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

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private void CheckEndProcess()
        {
            var checkEndProcessCriteria = new CheckEndProcessCriteria();
            IEnumerable<AutoEmployeePeriodModel> evaluationProcesses = evaluationService.CheckEndProcess(checkEndProcessCriteria);

            if (evaluationProcesses != null && evaluationProcesses.Any())
            {
                var updateEndProcessCriteria = new UpdateIsEndProcessCriteria()
                {
                    EvaluationProcesses = evaluationProcesses
                };

                CommandResult updateIsEndProcess = evaluationService.UpdateEndProcess(updateEndProcessCriteria);
                string messageResult = updateIsEndProcess.IsSuccessful ? "0" : updateIsEndProcess.Message;
                Log.Info("Result of update status of processes that ended yesterday: " + messageResult);

                IList<AutoEmployeePeriodModel> semiEvaluationProcesses = evaluationProcesses
                    .Where(evaluationProcess => evaluationProcess.ProcessId == (int)ProcessType.Evaluator
                        && evaluationProcess.TargetTypeId == (int)TargetTypes.SemiAnnual
                        && evaluationProcess.HasMonthlyEPA).ToList();

                foreach (AutoEmployeePeriodModel evaluationProcess in semiEvaluationProcesses)
                {
                    var getEPAInfoCriteria = new GetEPAInfoByIdCriteria
                    {
                        EPAId = evaluationProcess.EPAId
                    };
                    EPAModel epaInfo = employeeEPAService.GetEPAInfoById(getEPAInfoCriteria);

                    var updateTotalScoreOfSemiHasMonthlyCriteria = new UpdateTotalScoreOfSemiHasMonthlyCriteria
                    {
                        EPAId = evaluationProcess.EPAId,
                        MonthlyEPA = epaInfo.MonthlyEPA
                    };
                    CommandResult commandResult = evaluationService.UpdateTotalScoreOfSemiHasMonthly(updateTotalScoreOfSemiHasMonthlyCriteria);
                    messageResult = commandResult.IsSuccessful ? "0" : commandResult.Message;
                    Log.Info("Result of update total score semi has monthly " + evaluationProcess.EPAId + " : " + messageResult);
                }

                foreach (AutoEmployeePeriodModel evaluationProcess in evaluationProcesses.Where(evaluationProcess => evaluationProcess.ProcessId >= (int)ProcessType.Reviewer))
                {
                    if (string.IsNullOrEmpty(evaluationProcess.TotalScore))
                    {
                        var getEvaluationInfoCriteria = new GetEPAInfoByIdCriteria
                        {
                            EPAId = evaluationProcess.EPAId
                        };
                        EPAModel evaluationInfo = employeeEPAService.GetEPAInfoById(getEvaluationInfoCriteria);

                        float averageScore = GetAverageScorePreProcess(evaluationInfo, evaluationProcess.ProcessId);
                        Log.Info("Result of get average score of pre-process of EPA " + evaluationProcess.EPAId + " : " + averageScore);

                        var updateTotalScoreCriteria = new UpdateTotalScoreOfProcessCriteria
                        {
                            EPAId = evaluationProcess.EPAId,
                            ProcessId = evaluationProcess.ProcessId,
                            AverageScore = EncodeService.Encode(averageScore.ToString())
                        };
                        CommandResult commandResult = evaluationService.UpdateTotalScoreOfProcess(updateTotalScoreCriteria);
                        messageResult = commandResult.IsSuccessful ? "0" : commandResult.Message;
                        Log.Info("Result of update total score of process of EPA " + evaluationProcess.EPAId + " : " + messageResult);
                    }
                }
            }
        }

        private void CheckEndEvaluationPeriod()
        {
            var getAllEndedEvaluationCriteria = new GetEPAEndEvaluationPeriodCriteria();
            IEnumerable<EPAModel> allEvaluations = employeeEPAService.GetEPAEndEvaluationPeriod(getAllEndedEvaluationCriteria);

            if (allEvaluations != null && allEvaluations.Any())
            {
                UpdateFinalScoreAndSendMail(allEvaluations);
            }
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private void UpdateFinalScoreAndSendMail(IEnumerable<EPAModel> evaluationInfos)
        {
            IList<int> periods = new List<int>();

            var criteria = new GetStandardGradeCriteria();
            IEnumerable<StandardGradeModel> standardGrades = employeeEPAService.GetStandardGrade(criteria);

            foreach (EPAModel evaluationInfo in evaluationInfos)
            {
                float averagePerformanceScore = GetFinalScoreOfEvalution(evaluationInfo);
                Log.Info("Result of get average performance score of EPA " + evaluationInfo.EPAId + " :  " + averagePerformanceScore);

                float finalScore = averagePerformanceScore + evaluationInfo.DisciplineScore;
                finalScore = finalScore >= 0 ? finalScore : 0;

                evaluationInfo.Grade = employeeEPAService.ConvertScoreToGrade(finalScore, standardGrades);
                Log.Info("Result of get grade of final score of EPA " + evaluationInfo.EPAId + ": " + evaluationInfo.Grade);

                EncodeAndUpdateFinalScore(averagePerformanceScore, evaluationInfo.DisciplineScore, evaluationInfo.EPAId, evaluationInfo.Grade);

                if (AppSetting.SendMailEndPeriod)
                {
                    SendEndEvaluationMailToSelf(evaluationInfo, averagePerformanceScore, evaluationInfo.DisciplineScore);

                    if (evaluationInfo.Period != null)
                    {
                        if (evaluationInfo.Period.TargetTypeId == (int)TargetTypes.Probation
                            || evaluationInfo.Period.TargetTypeId == (int)TargetTypes.Promotion)
                        {
                            autoMailService.SendEndEvaluationMailToHumanResourceByEmployee(evaluationInfo);
                        }
                        else if ((evaluationInfo.Period.TargetTypeId == (int)TargetTypes.SemiAnnual || evaluationInfo.Period.TargetTypeId == (int)TargetTypes.Monthly)
                            && !periods.Contains(evaluationInfo.Period.PeriodId))
                        {
                            periods.Add(evaluationInfo.Period.PeriodId);
                        }
                    }
                }
            }

            if (AppSetting.SendMailEndPeriod && periods.Any())
            {
                foreach (int periodId in periods)
                {
                    var checkLastestProcessCriteria = new CheckLastestProcessOfPeriodCriteria
                    {
                        PeriodId = periodId
                    };
                    int lastestProcessOfPeriod = evaluationService.CheckLastestProcessOfPeriod(checkLastestProcessCriteria);
                    Log.Info("Result of check the end of period " + periodId + " : " + lastestProcessOfPeriod);

                    if (lastestProcessOfPeriod == 1)
                    {
                        evaluationService.GetAndSendEmailToAllRelatedEmployee(periodId, string.Empty, (int)ProcessType.HumanResource, (int)MailMode.EndPeriod);
                    }
                }
            }
        }

        private void SendEndEvaluationMailToSelf(EPAModel evaluationInfo, float averagePerformanceScore, float disciplineScore)
        {
            autoMailService.SendEndEvaluationMailToSelf(evaluationInfo, averagePerformanceScore, disciplineScore);
        }

        private void CheckSendNoticeAtBeginEmail()
        {
            var criteria = new GetPeriodHasStartedProcessCriteria();
            IEnumerable<AutoEmployeePeriodModel> startedProcess = periodService.GetPeriodHasStartedProcess(criteria);

            foreach (AutoEmployeePeriodModel periodProcess in startedProcess)
            {
                evaluationService.GetAndSendEmailToAllRelatedEmployee(periodProcess.PeriodId, string.Empty, periodProcess.ProcessId, (int)MailMode.Start);
            }
        }

        private void CheckSendReminderEmail()
        {
            var criteria = new GetPeriodHasReminderProcessCriteria();
            IEnumerable<AutoEmployeePeriodModel> reminderProcess = evaluationService.GetPeriodHasReminderProcess(criteria);

            foreach (AutoEmployeePeriodModel periodProcess in reminderProcess)
            {
                if (periodProcess.TargetTypeId == (int)TargetTypes.SemiAnnual
                    && (periodProcess.ProcessId == (int)ProcessType.Self || periodProcess.ProcessId == (int)ProcessType.Evaluator)
                    && periodProcess.HasMonthlyEPA)
                {
                    continue;
                }
                else
                {
                    evaluationService.GetAndSendEmailToAllRelatedEmployee(periodProcess.PeriodId, string.Empty, periodProcess.ProcessId, (int)MailMode.Reminder);
                }
            }
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
            string finalScoreEncode = EncodeService.Encode(finalScore.ToString());
            Log.Info("Result of encode final score of EPA " + evaluationId + ": " + finalScoreEncode);

            string gradeEncode = EncodeService.Encode(grade);
            Log.Info("Result of encode grade of EPA " + evaluationId + ": " + gradeEncode);

            var criteria = new UpdateFinalScoreEndEvaluationCriteria();
            criteria.EvaluationScores = criteria.ConvertToXml(evaluationId, finalScoreEncode, disciplineScore, gradeEncode);

            CommandResult commandResult = evaluationService.UpdateFinalScoreEndEvaluation(criteria);
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