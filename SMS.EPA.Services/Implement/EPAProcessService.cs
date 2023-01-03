namespace SMS.EPA.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Extensions;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Model;

    public interface IEPAProcessService
    {
        void UpdateTotalScoreOfProcess(int periodId, string employeeIds = null);

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
                UpdateTotalScoreOfProcess(processes);
            }
        }

        public void UpdateTotalScoreOfProcess(IEnumerable<AutoEvaluationProcessModel> processes)
        {
            UpdateTotalScoreOfSemiHasMonthly(processes.Where(x => x.ProcessId == (byte)ProcessType.Evaluator && x.IsSemiHasMonthly).ToList());

            var reviewerManagements = processes.Where(x => x.ProcessId >= (byte)ProcessType.Reviewer).ToList();
            foreach (AutoEvaluationProcessModel process in reviewerManagements)
            {
                if (string.IsNullOrWhiteSpace(process.TotalScore) || process.TotalScore.Equals("0"))
                {
                    float? averageScore = GetAverageScorePreProcess(processes.Where(x => x.EPAId == process.EPAId).ToList(), process.ProcessId);
                    float? adjustScore = !string.IsNullOrWhiteSpace(process.AdjustScore) ? float.Parse(process.AdjustScore) : (float?)null;
                    averageScore = CalculateTotalScoreOfProcess(averageScore, adjustScore);

                    process.TotalScore = averageScore.HasValue ? averageScore.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
                    process.IsChanged = true;
                }
            }

            processes = processes.Where(x => x.IsChanged).ToList();
            foreach(var process in processes)
            {
                process.TotalScore = cryptographyService.Encrypt(process.TotalScore?.ToString());
            }

            var criteria = new UpdateTotalScoreOfProcessCriteria
            {
                DataXml = processes.Where(x => x.IsChanged).ToXML().ToString()
            };
            baseService.ExecuteNonQuery(criteria);
        }

        private void UpdateTotalScoreOfSemiHasMonthly(IEnumerable<AutoEvaluationProcessModel> processes)
        {
            var epaInfos = new List<EPAModel>();
            foreach (var process in processes)
            {
                EPAModel epaInfo;
                if (epaInfos.Any(x => x.EPAId == process.EPAId))
                {
                    epaInfo = epaInfos.First(x => x.EPAId == process.EPAId);
                }
                else
                {
                    epaInfo = employeeEPAService.GetEPAInfoById(process.EPAId);
                    epaInfos.Add(epaInfo);
                }

                switch (process.ProcessId)
                {
                    case (byte)ProcessType.Self:
                        process.TotalScore = epaInfo.MonthlyEPA.AverageSelfScoreConverted?.ToString();
                        process.IsChanged = true;
                        break;

                    case (byte)ProcessType.Evaluator:
                        process.TotalScore = epaInfo.MonthlyEPA.AverageEvaluatorScoreConverted?.ToString();
                        process.IsChanged = true;
                        break;

                    case (byte)ProcessType.Reviewer:
                        float? averageScore = epaInfo.MonthlyEPA.AverageReviewerScoreConverted;
                        float? adjustScore = !string.IsNullOrWhiteSpace(process.AdjustScore) ? float.Parse(process.AdjustScore) : (float?)null;
                        averageScore = CalculateTotalScoreOfProcess(averageScore, adjustScore);

                        process.TotalScore = averageScore.HasValue ? averageScore.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
                        process.IsChanged = true;
                        break;

                    default:
                        // nothing
                        break;
                }
            }
        }

        private static float? CalculateTotalScoreOfProcess(float? averageScore = null, float? adjustScore = null)
        {
            if (averageScore.HasValue)
            {
                return adjustScore.HasValue ? averageScore.Value + adjustScore.Value : averageScore;
            }

            return adjustScore;
        }

        private static float? CalculateAverageScore(IList<AutoEvaluationProcessModel> processes)
        {
            float average = 0;
            float? result = null;

            var validProcesses = processes.Where(x => !string.IsNullOrWhiteSpace(x.TotalScore)).ToList();
            if (validProcesses.Any())
            {
                average += validProcesses.Sum(process => float.Parse(process.TotalScore));

                result = (float)Math.Round(average / validProcesses.Count, Constants.NumberOfDecimalDefault);
            }

            return result;
        }

        private static float? GetAverageScorePreProcess(IEnumerable<AutoEvaluationProcessModel> processes, byte processId)
        {
            float? averageScore;
            switch (processId)
            {
                case (byte)ProcessType.Reviewer:
                    averageScore = CalculateAverageScore(processes.Where(x => x.ProcessId == (byte)ProcessType.Evaluator).ToList());
                    break;

                case (byte)ProcessType.Management:
                    averageScore = CalculateAverageScore(processes.Where(x => x.ProcessId == (byte)ProcessType.Reviewer).ToList());
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