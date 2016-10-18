namespace SMS.EPA.AutoService.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using log4net;
    using SMS.EPA.AutoService.Criteria;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class EPAService : IEPAService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IEvaluationService evaluationService;
        private readonly IPeriodService periodService;
        private readonly IAutoMailService autoMailService;
        private readonly IEmployeeEPAService employeeEPAService;
        private readonly IMonthlyService monthlyService;
        private readonly ISemiAnnualService semiAnnualService;
        private readonly IProbationService probationService;
        private readonly IPromotionService promotionService;
        private readonly ICryptographyService cryptographyService;

        public EPAService()
            : this(null, null, null, null, null, null, null, null, null)
        {
        }

        public EPAService(
            ICryptographyService cryptographyService,
            IEvaluationService evaluationService,
            IPeriodService periodService,
            IAutoMailService autoMailService,
            IEmployeeEPAService employeeEPAService,
            IProbationService probationService,
            IMonthlyService monthlyService,
            ISemiAnnualService semiAnnualService,
            IPromotionService promotionService)
        {
            this.cryptographyService = cryptographyService ?? new CryptographyService();
            this.evaluationService = evaluationService ?? new EvaluationService();
            this.periodService = periodService ?? new PeriodService();
            this.autoMailService = autoMailService ?? new AutoMailService();
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            this.monthlyService = monthlyService ?? new MonthlyService();
            this.semiAnnualService = semiAnnualService ?? new SemiAnnualService();
            this.probationService = probationService ?? new ProbationService();
            this.promotionService = promotionService ?? new PromotionService();
        }

        public void Run()
        {
            try
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
            catch (Exception ex)
            {
                Log.Error("Error: " + ex.InnerException.Message);
            }
        }

        private void CheckSendMail()
        {
            Task.Factory.StartNew(CheckSendNoticeAtBeginEmail);

            Task.Factory.StartNew(CheckSendReminderEmail);
        }

        private void CheckEndProcess()
        {
            IEnumerable<AutoEmployeePeriodModel> evaluationProcesses = evaluationService.CheckEndProcess();

            if (evaluationProcesses != null && evaluationProcesses.Any())
            {
                UpdateEndProcess(evaluationProcesses);
                UpdateTotalScoreOfSemiHasMonthly(evaluationProcesses.Where(x => x.ProcessId == (int)ProcessType.Evaluator && x.IsSemiHasMonthly));
                UpdateTotalScoreOfProcess(evaluationProcesses.Where(x => x.ProcessId >= (int)ProcessType.Reviewer));
            }
        }

        private void UpdateEndProcess(IEnumerable<AutoEmployeePeriodModel> evaluationProcesses)
        {
            var criteria = new UpdateIsEndProcessCriteria()
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

        private void UpdateTotalScoreOfSemiHasMonthly(IEnumerable<AutoEmployeePeriodModel> evaluationProcesses)
        {
            var messageResult = string.Empty;

            foreach (AutoEmployeePeriodModel evaluationProcess in evaluationProcesses)
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

                messageResult = commandResult.IsSuccessful ? "0" : commandResult.Message;
                Log.Info("Result of update total score semi has monthly " + evaluationProcess.EPAId + " : " + messageResult);
            }
        }

        private void UpdateTotalScoreOfProcess(IEnumerable<AutoEmployeePeriodModel> evaluationProcesses)
        {
            var messageResult = string.Empty;

            foreach (AutoEmployeePeriodModel evaluationProcess in evaluationProcesses)
            {
                if (string.IsNullOrEmpty(evaluationProcess.TotalScore))
                {
                    EPAModel epaInfo = employeeEPAService.GetEPAInfoById(evaluationProcess.EPAId);

                    float averageScore = GetAverageScorePreProcess(epaInfo, evaluationProcess.ProcessId);
                    Log.Info("Result of get average score of pre-process of EPA " + evaluationProcess.EPAId + " : " + averageScore);

                    var criteria = new UpdateTotalScoreOfProcessCriteria
                    {
                        EPAId = evaluationProcess.EPAId,
                        ProcessId = evaluationProcess.ProcessId,
                        AverageScore = cryptographyService.Encrypt(averageScore.ToString())
                    };
                    var commandResult = new CommandResult();

                    if (criteria.IsValid())
                    {
                        commandResult = evaluationService.UpdateTotalScoreOfProcess(criteria);
                    }

                    messageResult = commandResult.IsSuccessful ? "0" : commandResult.Message;
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
                float averagePerformanceScore = GetFinalScoreOfEvalution(epaInfo);
                Log.Info("Result of get average performance score of EPA " + epaInfo.EPAId + " :  " + averagePerformanceScore);

                float finalScore = averagePerformanceScore + epaInfo.DisciplineScore;
                finalScore = finalScore >= 0 ? finalScore : 0;

                epaInfo.Grade = employeeEPAService.ConvertScoreToGrade(finalScore, standardGrades);
                Log.Info("Result of get grade of final score of EPA " + epaInfo.EPAId + ": " + epaInfo.Grade);

                EncryptAndUpdateFinalScore(averagePerformanceScore, epaInfo.DisciplineScore, epaInfo.EPAId, epaInfo.Grade);

                if (AppSetting.SendMailEndPeriod)
                {
                    autoMailService.SendEndEvaluationMailToSelf(epaInfo, averagePerformanceScore, epaInfo.DisciplineScore);

                    if (epaInfo.Period != null)
                    {
                        if (epaInfo.IsProbation || epaInfo.IsPromotion)
                        {
                            autoMailService.SendEndEvaluationMailToHumanResourceByEmployee(epaInfo);
                        }
                        else if ((epaInfo.IsSemiAnnual || epaInfo.IsMonthly) && !periods.Contains(epaInfo.Period.PeriodId))
                        {
                            periods.Add(epaInfo.Period.PeriodId);
                        }
                    }
                }
            }

            SendMailEndPeriod(periods);
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
                if (periodProcess.IsSemiHasMonthly
                    && (periodProcess.ProcessId == (int)ProcessType.Self || periodProcess.ProcessId == (int)ProcessType.Evaluator))
                {
                    continue;
                }
                else
                {
                    evaluationService.GetAndSendEmailToAllRelatedEmployee(
                        periodProcess.PeriodId,
                        string.Empty,
                        periodProcess.ProcessId,
                        (int)MailMode.Reminder);
                }
            }
        }

        private float GetFinalScoreOfEvalution(EPAModel epaInfo)
        {
            float finalScore = 0;

            if (epaInfo.Process.Managements.Any())
            {
                finalScore = epaInfo.Process.AverageScoreByManagements;
            }
            else if (epaInfo.Process.Reviewers.Any())
            {
                finalScore = epaInfo.Process.AverageScoreByReviewers;
            }
            else if (epaInfo.Process.Evaluators.Any())
            {
                finalScore = epaInfo.Process.AverageScoreByEvaluators;
            }

            return finalScore;
        }

        private void EncryptAndUpdateFinalScore(float performanceScore, float disciplineScore, int evaluationId, string grade)
        {
            float finalScore = performanceScore + disciplineScore;
            string finalScoreEncrypt = cryptographyService.Encrypt(finalScore.ToString());
            Log.Info("Result of encrypt final score of EPA " + evaluationId + ": " + finalScoreEncrypt);

            string gradeEncrypt = cryptographyService.Encrypt(grade);
            Log.Info("Result of encrypt grade of EPA " + evaluationId + ": " + gradeEncrypt);

            var criteria = new UpdateFinalScoreEndEvaluationCriteria();
            criteria.EvaluationScores = criteria.ConvertToXml(evaluationId, finalScoreEncrypt, disciplineScore, gradeEncrypt);

            CommandResult commandResult = evaluationService.UpdateFinalScoreEndEvaluation(criteria);

            string messageResult = commandResult.IsSuccessful ? "0" : commandResult.Message;
            Log.Info("Result of update final score and discipline score of EPA " + evaluationId + ": " + messageResult);
        }

        private float GetAverageScorePreProcess(EPAModel epaInfo, int processId)
        {
            float averageScore = 0;

            switch (processId)
            {
                case (int)ProcessType.Reviewer:
                    averageScore = epaInfo.Process.AverageScoreByEvaluators;
                    break;

                case (int)ProcessType.Management:
                    averageScore = epaInfo.Process.Reviewers.Any() ? epaInfo.Process.AverageScoreByReviewers : epaInfo.Process.AverageScoreByEvaluators;
                    break;
            }

            return averageScore;
        }
    }
}