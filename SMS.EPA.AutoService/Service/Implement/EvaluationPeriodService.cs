namespace SMS.EPA.AutoService.Service
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Services;
    using Criteria;
    using EPA.Criteria;
    using Model;
    using Services.Implement;
    using Services.Interfaces;

    public class EvaluationPeriodService : IEvaluationPeriodService
    {
        private readonly IEmployeeEPAService employeeEPAService;
        private readonly ICryptographyService cryptographyService;
        private readonly IPeriodService periodService;
        private readonly IPeriodMailService periodMailService;
        private readonly IBaseService baseService;

        public EvaluationPeriodService() : this(null, null, null, null, null)
        {
        }

        public EvaluationPeriodService(
            ICryptographyService cryptographyService,
            IEmployeeEPAService employeeEPAService,
            IPeriodService periodService,
            IPeriodMailService periodMailService,
            IBaseService baseService)
        {
            this.cryptographyService = cryptographyService ?? new CryptographyService();
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            this.periodService = periodService ?? new PeriodService();
            this.periodMailService = periodMailService ?? new PeriodMailService();
            this.baseService = baseService ?? new BaseService();
        }

        public void CheckEndEvaluationPeriod()
        {
            var criteria = new GetEPAEndEvaluationPeriodCriteria();
            IEnumerable<EPAModel> epaInfos = employeeEPAService.GetEPAEndEvaluationPeriod(criteria);

            if (epaInfos != null && epaInfos.Any())
            {
                UpdateFinalScoreAndSendMail(epaInfos);
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

        private void UpdateFinalScoreAndSendMail(IEnumerable<EPAModel> epaInfos)
        {
            IList<int> periods = new List<int>();
            IEnumerable<StandardGradeModel> standardGrades = baseService.GetData<StandardGradeModel>(new GetStandardGradeCriteria());

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
            periodMailService.SendEndEvaluationMailToSelf(epaInfo, averagePerformanceScore, epaInfo.DisciplineScore);

            if (epaInfo.Period != null)
            {
                if (epaInfo.IsProbation || epaInfo.IsPromotion)
                {
                    periodMailService.SendEndEvaluationMailToHumanResourceByEmployee(epaInfo);
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
                    int lastestProcessOfPeriod = CheckLastestProcessOfPeriod(periodId);

                    if (lastestProcessOfPeriod == 1)
                    {
                        GetAndSendEmailToAllRelatedEmployee(
                            periodId,
                            string.Empty,
                            (int)ProcessType.HumanResource,
                            (int)MailMode.EndPeriod);
                    }
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

            if (finalScore.HasValue)
            {
                finalScore = finalScore.Value > Constants.MaxFinalScore ? Constants.MaxFinalScore : finalScore.Value;
            }

            string finalScoreEncrypt = finalScore.HasValue
                ? cryptographyService.Encrypt(finalScore.Value.ToString(CultureInfo.InvariantCulture))
                : string.Empty;

            string gradeEncrypt = cryptographyService.Encrypt(grade);

            var criteria = new UpdateFinalScoreEndEvaluationCriteria
            {
                EPAId = evaluationId,
                FinalScore = finalScoreEncrypt,
                Grade = gradeEncrypt,
                DisciplineScore = disciplineScore
            };

            if (criteria.IsValid())
            {
                baseService.ExecuteNonQuery(criteria);
            }
        }

        private int CheckLastestProcessOfPeriod(int periodId)
        {
            var criteria = new CheckLastestProcessOfPeriodCriteria
            {
                PeriodId = periodId
            };
            int lastestProcess = 0;

            if (criteria.IsValid())
            {
                lastestProcess = baseService.GetDataEntity<int>(criteria);
            }

            return lastestProcess;
        }

        private void GetAndSendEmailToAllRelatedEmployee(int periodId, string employeeIds, int? processId, int mailMode)
        {
            PeriodModel period = periodService.GetPeriodById(periodId);
            IEnumerable<PeriodFlowModel> employeeSettings = GetEmployeeSettings(periodId, employeeIds, processId);

            switch (mailMode)
            {
                case (int)MailMode.Start:
                    periodMailService.SendReminderMail(period, employeeSettings, (int)MailMode.Start);
                    break;

                case (int)MailMode.Reminder:
                    periodMailService.SendReminderMail(period, employeeSettings, (int)MailMode.Reminder);
                    break;

                case (int)MailMode.EndPeriod:
                    periodMailService.SendEndEvaluationMailToHumanResourceByPeriod(period, employeeSettings);
                    break;

                default:
                    return;
            }
        }

        private IEnumerable<PeriodFlowModel> GetEmployeeSettings(int periodId, string employeeIds, int? processId)
        {
            var criteria = new GetEmployeeSettingByPeriodIdCriteria
            {
                PeriodId = periodId,
                EmployeeIds = employeeIds,
                ProcessId = processId
            };

            IEnumerable<PeriodFlowModel> employeeSettings = periodService.GetEmployeeSettingByPeriodId(criteria);

            return employeeSettings;
        }
    }
}