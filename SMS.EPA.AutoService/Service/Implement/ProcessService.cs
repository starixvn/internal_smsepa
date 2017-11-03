namespace SMS.EPA.AutoService.Service
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Core.Enumerations;
    using Core.Services;
    using Criteria;
    using Model;
    using Services.Implement;
    using Services.Interfaces;

    public class ProcessService : BaseService, IProcessService
    {
        private readonly IEmployeeEPAService employeeEPAService;
        private readonly ICryptographyService cryptographyService;

        public ProcessService() : this(null, null)
        {
        }

        public ProcessService(
            ICryptographyService cryptographyService,
            IEmployeeEPAService employeeEPAService)
        {
            this.cryptographyService = cryptographyService ?? new CryptographyService();
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
        }

        public void UpdateInfomationOfProcess()
        {
            IEnumerable<AutoEvaluationProcessModel> evaluationProcesses = CheckEndProcess();

            if (evaluationProcesses != null && evaluationProcesses.Any())
            {
                UpdateEndProcess(evaluationProcesses);
                UpdateTotalScoreOfSemiHasMonthly(evaluationProcesses.Where(x => x.ProcessId == (int)ProcessType.Evaluator && x.IsSemiHasMonthly));
                UpdateTotalScoreOfProcess(evaluationProcesses.Where(x => x.ProcessId >= (int)ProcessType.Reviewer));
            }
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

        private void UpdateEndProcess(IEnumerable<AutoEvaluationProcessModel> evaluationProcesses)
        {
            var criteria = new UpdateIsEndProcessCriteria
            {
                EvaluationProcesses = evaluationProcesses
            };

            if (criteria.IsValid())
            {
                Repository.ExecuteNonQuery(criteria);
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
                    Repository.ExecuteNonQuery(criteria);
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
                        Repository.ExecuteNonQuery(criteria);
                    }
                }
            }
        }

        private IEnumerable<AutoEvaluationProcessModel> CheckEndProcess()
        {
            var criteria = new CheckEndProcessCriteria();
            IEnumerable<AutoEvaluationProcessModel> evaluationProcesses = new List<AutoEvaluationProcessModel>();

            if (criteria.IsValid())
            {
                evaluationProcesses = GetData<AutoEvaluationProcessModel>(criteria);
                DecryptTotalScore(evaluationProcesses);
            }

            return evaluationProcesses;
        }

        private void DecryptTotalScore(IEnumerable<AutoEvaluationProcessModel> evaluationProcesses)
        {
            foreach (AutoEvaluationProcessModel evaluationProcess in evaluationProcesses)
            {
                evaluationProcess.TotalScore = cryptographyService.Decrypt(evaluationProcess.TotalScore);
            }
        }
    }
}