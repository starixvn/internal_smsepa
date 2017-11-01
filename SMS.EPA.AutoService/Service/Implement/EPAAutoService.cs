namespace SMS.EPA.AutoService.Service.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Services;
    using Criteria;
    using EPA.Criteria;
    using Fanex.Logging;
    using Interfaces;
    using Model;
    using Service;
    using Services.Implement;
    using Services.Interfaces;

    public class EPAAutoService : IEPAAutoService
    {
        private readonly IEvaluationService evaluationService;
        private readonly IPeriodService periodService;
        private readonly IPeriodMailService reminderMailService;
        private readonly IEmployeeEPAService employeeEPAService;
        private readonly IMonthlyService monthlyService;
        private readonly ISemiAnnualService semiAnnualService;
        private readonly IProbationService probationService;
        private readonly IPromotionService promotionService;
        private readonly ICryptographyService cryptographyService;

        public EPAAutoService()
            : this(null, null, null, null, null, null, null, null, null)
        {
        }

#pragma warning disable S107 // Methods should not have too many parameters
        public EPAAutoService(
            IEvaluationService evaluationService,
            IPeriodService periodService,
            IPeriodMailService reminderMailService,
            IEmployeeEPAService employeeEPAService,
            IMonthlyService monthlyService,
            ISemiAnnualService semiAnnualService,
            IProbationService probationService,
            IPromotionService promotionService,
            ICryptographyService cryptographyService)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            this.cryptographyService = cryptographyService ?? new CryptographyService();
            this.evaluationService = evaluationService ?? new EvaluationService();
            this.periodService = periodService ?? new PeriodService();
            this.reminderMailService = reminderMailService ?? new PeriodMailService();
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
                if (ex.InnerException != null)
                {
                    LogService.Error(ex.InnerException.Message, ex);
                }
            }
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

            if (criteria.IsValid())
            {
                evaluationService.UpdateEndProcess(criteria);
            }
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

                if (criteria.IsValid())
                {
                    evaluationService.UpdateTotalScoreOfSemiHasMonthly(criteria);
                }
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

                    var criteria = new UpdateTotalScoreOfProcessCriteria
                    {
                        EPAId = evaluationProcess.EPAId,
                        ProcessId = evaluationProcess.ProcessId,
                        AverageScore = averageScore.HasValue
                            ? cryptographyService.Encrypt(averageScore.Value.ToString(CultureInfo.InvariantCulture))
                            : string.Empty
                    };

                    if (criteria.IsValid())
                    {
                        evaluationService.UpdateTotalScoreOfProcess(criteria);
                    }
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

                epaInfo.Grade = performanceScore.HasValue
                    ? employeeEPAService.ConvertScoreToGrade(performanceScore, epaInfo.DisciplineScore, epaInfo.HasDisciplineScore, standardGrades)
                    : string.Empty;

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

            string gradeEncrypt = cryptographyService.Encrypt(grade);

            var criteria = new UpdateFinalScoreEndEvaluationCriteria
            {
                EvaluationScores = UpdateFinalScoreEndEvaluationCriteria.ConvertToXml(
                    evaluationId,
                    finalScoreEncrypt,
                    disciplineScore,
                    gradeEncrypt)
            };

            evaluationService.UpdateFinalScoreEndEvaluation(criteria);
        }
    }
}