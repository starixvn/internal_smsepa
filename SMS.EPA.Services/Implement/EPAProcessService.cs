namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Interfaces;

    public interface IEPAProcessService
    {
        void UpdateTotalScoreOfProcess(int periodId, string employeeIds = null);

        void UpdateTotalScoreOfSemiHasMonthly(IEnumerable<AutoEvaluationProcessModel> processes);

        void UpdateTotalScoreOfProcess(IEnumerable<AutoEvaluationProcessModel> processes);
    }

    public class EPAProcessService : IEPAProcessService
    {
        private readonly IEmployeeEPAService employeeEPAService;
        private readonly ICryptographyService cryptographyService;
        private readonly IBaseService baseService;

        public EPAProcessService() : this(null, null, null)
        {
        }

        public EPAProcessService(
            ICryptographyService cryptographyService,
            IEmployeeEPAService employeeEPAService,
            IBaseService baseService)
        {
            this.cryptographyService = cryptographyService ?? new CryptographyService();
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            this.baseService = baseService ?? new BaseService();
        }

        public void UpdateTotalScoreOfProcess(int periodId, string employeeIds = null)
        {
            var criteria = new GetDataToUpdateTotalScoreOfProcessCriteria
            {
                PeriodId = periodId,
                EmployeeIds = employeeIds
            };
            var processes = baseService.GetData<AutoEvaluationProcessModel>(criteria);

            if (processes.Any())
            {
                DecryptTotalScore(processes);
                UpdateTotalScoreOfSemiHasMonthly(processes.Where(x => x.ProcessId == (byte)ProcessType.Evaluator && x.IsSemiHasMonthly));
                UpdateTotalScoreOfProcess(processes.Where(x => x.ProcessId >= (byte)ProcessType.Reviewer));
            }
        }

        public void UpdateTotalScoreOfSemiHasMonthly(IEnumerable<AutoEvaluationProcessModel> processes)
        {
            foreach (AutoEvaluationProcessModel process in processes)
            {
                EPAModel epaInfo = employeeEPAService.GetEPAInfoById(process.EPAId);
                var criteria = new UpdateTotalScoreOfSemiHasMonthlyCriteria
                {
                    EPAId = process.EPAId,
                    MonthlyEPA = epaInfo.MonthlyEPA
                };
                baseService.ExecuteNonQuery(criteria);
            }
        }

        public void UpdateTotalScoreOfProcess(IEnumerable<AutoEvaluationProcessModel> processes)
        {
            foreach (AutoEvaluationProcessModel process in processes)
            {
                if (string.IsNullOrEmpty(process.TotalScore))
                {
                    EPAModel epaInfo = employeeEPAService.GetEPAInfoById(process.EPAId);
                    float? averageScore = GetAverageScorePreProcess(epaInfo, process.ProcessId);
                    float? adjustScore = !string.IsNullOrWhiteSpace(process.AdjustScore) ? float.Parse(process.AdjustScore) : (float?)null;
                    averageScore += adjustScore;

                    var criteria = new UpdateTotalScoreOfProcessCriteria
                    {
                        EPAId = process.EPAId,
                        ProcessId = process.ProcessId,
                        AverageScore = averageScore.HasValue
                            ? cryptographyService.Encrypt(averageScore.Value.ToString(CultureInfo.InvariantCulture))
                            : string.Empty
                    };
                    baseService.ExecuteNonQuery(criteria);
                }
            }
        }

        private static float? GetAverageScorePreProcess(EPAModel epaInfo, int processId)
        {
            float? averageScore;
            switch (processId)
            {
                case (byte)ProcessType.Reviewer:
                    averageScore = epaInfo.Process.AverageScoreByEvaluators;
                    break;

                case (byte)ProcessType.Management:
                    averageScore = epaInfo.Process.AverageScoreByReviewers;
                    break;

                default:
                    averageScore = null;
                    break;
            }

            return averageScore;
        }

        private void DecryptTotalScore(IEnumerable<AutoEvaluationProcessModel> processes)
        {
            foreach (AutoEvaluationProcessModel process in processes)
            {
                process.TotalScore = cryptographyService.Decrypt(process.TotalScore);
                process.AdjustScore = cryptographyService.Decrypt(process.AdjustScore);
            }
        }
    }
}