namespace SMS.EPA.AutoService.Service
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using SMS.EPA.AutoService.Criteria;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Model.AutoService;
    using SMS.EPA.Services;

    public class EvaluationPeriodService : IEvaluationPeriodService
    {
        private readonly IEmployeeEPAService employeeEPAService;
        private readonly ICryptographyService cryptographyService;
        private readonly IPeriodService periodService;
        private readonly IPeriodMailService periodMailService;
        private readonly IBaseService baseService;
        private static readonly log4net.ILog logfile = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
            var standardGrades = baseService.GetData<StandardGradeModel>(new GetStandardGradeCriteria());

            foreach (EPAModel epaInfo in epaInfos)
            {
                float? performanceScore = GetFinalScoreOfEvalution(epaInfo);

                logfile.InfoFormat("EPAId: {0}\nPerformanceScore: {1}\nHasDisciplineScore: {2}\nDisciplineScore: {3}",
                    epaInfo.EPAId,
                    performanceScore,
                    epaInfo.DisciplineScore,
                    epaInfo.HasDisciplineScore);
                epaInfo.Grade = performanceScore.HasValue
                    ? employeeEPAService.ConvertScoreToGrade(performanceScore, epaInfo.DisciplineScore, epaInfo.HasDisciplineScore, standardGrades)
                    : string.Empty;

                EncryptAndUpdateFinalScore(performanceScore, epaInfo.DisciplineScore, epaInfo.EPAId, epaInfo.Grade);

                if (AppSetting.SendMailEndPeriod)
                {
                    SendMailEndEvaluation(periods, epaInfo, performanceScore);
                }
            }

            SendMailEndPeriodToHR(periods);
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

        private void SendMailEndPeriodToHR(IList<int> periods)
        {
            if (AppSetting.SendMailEndPeriod && periods.Any())
            {
                foreach (int periodId in periods)
                {
                    var criteria = new CheckLastestProcessOfPeriodCriteria
                    {
                        PeriodId = periodId
                    };
                    var isEndPeriod = baseService.GetDataEntity<bool>(criteria);
                    if (isEndPeriod)
                    {
                        IEnumerable<EmployeeSettingToSendMail> employeeSettings =
                            periodService.GetEmployeeSettingToSendMail(periodId, (byte)ProcessType.HumanResource);
                        periodMailService.SendEndEvaluationMailToHumanResourceByPeriod(employeeSettings);
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
    }
}