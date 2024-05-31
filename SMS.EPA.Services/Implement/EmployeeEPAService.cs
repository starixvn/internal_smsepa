namespace SMS.EPA.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using HRM.UserControl.Client.Core.Models.Role;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Core.Utilities;
    using SMS.EPA.Criteria;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Model;
    using SMS.EPA.Repositories;
    using SMS.EPA.Resources;

    public class EmployeeEPAService : IEmployeeEPAService
    {
        private readonly IEmployeeEPARepository employeeEPARepository;
        private readonly IProbationExtendedQuestionRepository extendedQuestionRepository;
        private readonly IBaseService baseService;
        private readonly ICommonService commonService;
        private readonly ICryptographyService cryptographyService;

        public EmployeeEPAService() : this(null, null, null, null, null)
        {
        }

        public EmployeeEPAService(
            IEmployeeEPARepository employeeEPARepository,
            IProbationExtendedQuestionRepository extendedQuestionRepository,
            IBaseService baseService,
            ICommonService commonService,
            ICryptographyService cryptographyService)
        {
            this.employeeEPARepository = employeeEPARepository ?? new EmployeeEPARepository();
            this.extendedQuestionRepository = extendedQuestionRepository ?? new ProbationExtendedQuestionRepository();
            this.cryptographyService = cryptographyService ?? new CryptographyService();
            this.baseService = baseService ?? new BaseService();
            this.commonService = commonService ?? new CommonService();
        }

        public EPAModel GetEPAInfoById(int epaId)
        {
            var criteria = new GetEPAInfoByIdCriteria
            {
                EPAId = epaId
            };

            var epaInfo = new EPAModel();

            if (criteria.IsValid())
            {
                epaInfo = employeeEPARepository.GetEPAInfo(criteria);

                if (epaInfo != null)
                {
                    epaInfo.JobDescriptions = commonService.GetJobDescriptionByEPAId(epaInfo.EPAId);
                    SpreadKPIScore(epaInfo.KPIGroups);

                    if (epaInfo.IsSemiHasMonthly)
                    {
                        epaInfo.MonthlyEPA.MonthlyEPAs = GetMonthlyEPAOfSemi(epaInfo.EPAId.ToString());
                        ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
                    }

                    if (epaInfo.IsProbation || epaInfo.IsPromotion || epaInfo.IsSemiAnnual)
                    {
                        epaInfo.Recommendations = GetRecommendationByEPAId(epaInfo.EPAId);
                    }

                    if (epaInfo.IsProbation)
                    {
                        epaInfo.ExtendedQuestion = GetExtendedQuestionByEPAId(epaInfo.EPAId);
                    }
                }
            }

            return epaInfo;
        }

        public void RemoveSensitiveDataOnEmployeeRole(EPAModel epaInfo, int currentEmployeeId, IList<Role> roles)
        {
            if (epaInfo == null)
            {
                return;
            }

            // Remove all data for open Process (process that not evaluated / already evaluated but are extended
            var processOfOtherActors = epaInfo.Process.Managements.Where(x => x.StatusId == (byte)ProcessStatusType.Open);
            processOfOtherActors = processOfOtherActors.Concat(epaInfo.Process.Reviewers.Where(x => x.StatusId == (byte)ProcessStatusType.Open));
            processOfOtherActors = processOfOtherActors.Concat(epaInfo.Process.Evaluators.Where(x => x.StatusId == (byte)ProcessStatusType.Open));

            RemoveSensitiveTotalScoreAndComment(processOfOtherActors);

            if (IsSA_HRRole(roles))
            {
                if (epaInfo.IsSemiHasMonthly)
                {
                    CalculateFinalScoreForSemiHasMonthly(epaInfo);
                }
                else
                {
                    CalculateFinalScore(epaInfo);
                }

                return;
            }

            // Remove if user is not the one who evalutate that EPA
            processOfOtherActors = RemoveProcessOfOtherActor(epaInfo.Process.Managements, currentEmployeeId);
            RemoveSensitiveTotalScoreAndComment(processOfOtherActors);

            processOfOtherActors = RemoveProcessOfOtherActor(epaInfo.Process.Reviewers, currentEmployeeId);
            RemoveSensitiveTotalScoreAndComment(processOfOtherActors);

            processOfOtherActors = RemoveProcessOfOtherActor(epaInfo.Process.Evaluators, currentEmployeeId);

            if (processOfOtherActors.Any())
            {
                foreach (var process in processOfOtherActors)
                {
                    process.TotalScore = string.Empty;
                    process.Comment = string.Empty;
                }

                var evaluationModels = epaInfo.KPIGroups.SelectMany(x => x.KPIs).SelectMany(x => x.EvaluationByEvaluators);
                foreach (var evaluationModel in evaluationModels)
                {
                    evaluationModel.Score = string.Empty;
                    evaluationModel.Rate = 0;
                    evaluationModel.Comment = Lang.NotAvailable;
                }
            }

            if (epaInfo.IsSemiHasMonthly)
            {
                CalculateFinalScoreForSemiHasMonthly(epaInfo);
            }
            else
            {
                CalculateFinalScore(epaInfo);
            }
        }

        public IEnumerable<EPAModel> GetEPAEndEvaluationPeriod(GetEPAEndEvaluationPeriodCriteria criteria)
        {
            IEnumerable<EPAModel> epaInfos = new List<EPAModel>();
            if (criteria != null && criteria.IsValid())
            {
                epaInfos = employeeEPARepository.GetEPAEndEvaluationPeriod(criteria);
            }

            return epaInfos;
        }

        public IEnumerable<EPARecommendation> GetRecommendationByEPAId(int epaId)
        {
            var criteria = new GetRecommendationByEPAIdCriteria
            {
                EPAId = epaId
            };
            return baseService.GetData<EPARecommendation>(criteria);
        }

        public EPAProbationExtendedQuestionModel GetExtendedQuestionByEPAId(int epaId)
        {
            var criteria = new GetExtendedQuestionCriteria
            {
                EPAId = epaId
            };

            var extendedQuestion = new EPAProbationExtendedQuestionModel();

            if (criteria.IsValid())
            {
                extendedQuestion = extendedQuestionRepository.GetExtendedQuestion(criteria);
            }

            return extendedQuestion;
        }

#pragma warning disable S1541 // Methods and properties should not be too complex
        public string ConvertScoreToGrade(
#pragma warning restore S1541 // Methods and properties should not be too complex
            float? performanceScore,
            float disciplineScore,
            bool hasDisciplineScore,
            IEnumerable<StandardGradeModel> standardGrades)
        {
            var grade = Lang.NotAvailable;

            if (performanceScore == null)
            {
                return grade;
            }

            float finalScore = performanceScore.Value + disciplineScore;

            var finalGrade = standardGrades
                .FirstOrDefault(x => x.Type == (int)GradeType.Final && x.LeftScore <= finalScore && x.RightScore >= finalScore);

            if (finalGrade != null)
            {
                grade = finalGrade.Code;
            }

            if (hasDisciplineScore)
            {
                var performanceGrade = standardGrades
                    .FirstOrDefault(x => x.Type == (int)GradeType.Performance && x.LeftScore <= performanceScore && x.RightScore >= performanceScore);

                var disciplineGrade = standardGrades
                    .FirstOrDefault(x => x.Type == (int)GradeType.Discipline && x.LeftScore <= disciplineScore && x.RightScore >= disciplineScore);

                if (performanceGrade == null || disciplineGrade == null)
                {
                    return Lang.NotAvailable;
                }

                if (performanceGrade.GradeId > (byte)StandardGrade.Average || disciplineGrade.GradeId > (byte)StandardGrade.Average)
                {
                    grade = string.Compare(performanceGrade.Code, disciplineGrade.Code, StringComparison.OrdinalIgnoreCase) > 0
                        ? performanceGrade.Code
                        : disciplineGrade.Code;
                }
            }

            return grade;
        }

        public IList<EPAModel> GetMonthlyEPAOfSemi(string epaIds)
        {
            var criteria = new GetMonthlyEPAOfSemiCriteria
            {
                EPAIds = epaIds
            };

            IList<EPAModel> monthlyEPAs = employeeEPARepository.GetEPA(criteria).ToList();

            return monthlyEPAs;
        }

        public IList<EPAModel> GetEPAToUpdateFinalScore(GetEPAToUpdateFinalScoreCriteria criteria)
        {
            IList<EPAModel> epaInfos = new List<EPAModel>();

            if (criteria != null && criteria.IsValid())
            {
                epaInfos = employeeEPARepository.GetEPAToUpdateFinalScore(criteria);
            }

            return epaInfos;
        }

        public CommandResult GetDataAndUpdateFinalScore(int modifiedBy, int periodId, string employeeIds = null)
        {
            var criteria = new GetDataToUpdateFinalScoreCriteria
            {
                PeriodId = periodId,
                EmployeeIds = employeeIds
            };
            IEnumerable<EPAModel> epaInfos = employeeEPARepository.GetEPAEndEvaluationPeriod(criteria);
            IEnumerable<StandardGradeModel> standardGrades = commonService.GetStandardGrade();
            foreach (EPAModel epaInfo in epaInfos)
            {
                float? performanceScore = GetFinalScoreOfEvalution(epaInfo);
                float? finalScore = performanceScore.HasValue ? (performanceScore.Value + epaInfo.DisciplineScore) : (float?)null;

                if (finalScore.HasValue)
                {
                    finalScore = finalScore.Value > Constants.MaxFinalScore ? Constants.MaxFinalScore : finalScore.Value;
                }

                epaInfo.FinalScore = finalScore.HasValue
                         ? cryptographyService.Encrypt(finalScore.Value.ToString(CultureInfo.InvariantCulture))
                         : string.Empty;
                string grade = performanceScore.HasValue
                    ? ConvertScoreToGrade(performanceScore, epaInfo.DisciplineScore, epaInfo.HasDisciplineScore, standardGrades)
                    : string.Empty;
                epaInfo.Grade = !string.IsNullOrWhiteSpace(grade) ? cryptographyService.Encrypt(grade) : string.Empty;
            }

            return baseService.ExecuteNonQuery(new UpdateFinalScoreEPAEndEvaluationCriteria
            {
                ModifiedBy = modifiedBy,
                EPAInfos = epaInfos
            });
        }

        public void GroupMonthlyEPABySemi(EPAModel semiEPA, IList<EPAModel> monthlyEPAs)
        {
            if (semiEPA != null && monthlyEPAs != null && monthlyEPAs.Count > 0)
            {
                foreach (EPAModel monthlyEPA in monthlyEPAs.Where(x => x.EPAParentId.Equals(semiEPA.EPAId)))
                {
                    semiEPA.MonthlyEPA.MonthlyEPAs.Add(monthlyEPA);
                }
            }
        }

        public void ConvertAverageMonthlyScore(MonthlyEPAModel monthlyEPA, ScoreRatingTypeModel scoreRatingType)
        {
            if (monthlyEPA.MonthlyEPAs.Any())
            {
                const byte numOfFragtionalDigits = 2;
                float averageScore;
                ScoreRatingTypeModel monthlyScoreRatingType = monthlyEPA.MonthlyEPAs[0].ScoreRatingType;

                if (monthlyEPA.AverageSelfScore.HasValue)
                {
                    averageScore = monthlyEPA.AverageSelfScore.Value * scoreRatingType.SelfEvaluated / monthlyScoreRatingType.SelfEvaluated;
                    monthlyEPA.AverageSelfScoreConverted = (float)Math.Round(averageScore, numOfFragtionalDigits);
                }
                else
                {
                    monthlyEPA.AverageSelfScoreConverted = null;
                }

                if (monthlyEPA.AverageEvaluatorScores.HasValue)
                {
                    averageScore = monthlyEPA.AverageEvaluatorScores.Value * scoreRatingType.Evaluator / monthlyScoreRatingType.Evaluator;
                    monthlyEPA.AverageEvaluatorScoreConverted = (float)Math.Round(averageScore, numOfFragtionalDigits);
                }
                else
                {
                    monthlyEPA.AverageEvaluatorScoreConverted = null;
                }

                if (monthlyEPA.AverageReviewerScores.HasValue)
                {
                    averageScore = monthlyEPA.AverageReviewerScores.Value * scoreRatingType.Reviewer / monthlyScoreRatingType.Reviewer;
                    monthlyEPA.AverageReviewerScoreConverted = (float)Math.Round(averageScore, numOfFragtionalDigits);
                }
                else
                {
                    monthlyEPA.AverageReviewerScoreConverted = null;
                }
            }
        }

        public void ExportEPAAfterUpdateFinalScore(string periodName, IList<EPAModel> epaInfos, string templatePath)
        {
            const byte headerRowIndex = 2;
            const byte numOfColumns = 9;

            if (epaInfos != null && epaInfos.Any())
            {
                var writer = new ExcelTemplateWriter(templatePath, "Update_Final_Score", "Update Final Score");

                // Fill report title.
                writer.WriteRow(0, new object[] { periodName });
                writer.CopyRows(1, 1);

                // Fill report header.
                writer.CopyRows(headerRowIndex, 1);

                FillContentRow(epaInfos, ref writer);
                writer.SyncColumnsWidth(numOfColumns);
                writer.CreateFreezePane(0, headerRowIndex + 1);

                writer.Render();
            }
        }

        public void SetMaxPerformanceScore(EPAModel epaInfo)
        {
            switch (epaInfo.Process.CurrentProcess.ProcessId)
            {
                case (byte)ProcessType.Self:
                    epaInfo.Process.CurrentProcess.MaxPerformanceScore = epaInfo.ScoreRatingType.SelfEvaluated;
                    break;

                case (byte)ProcessType.Evaluator:
                    epaInfo.Process.CurrentProcess.MaxPerformanceScore = epaInfo.ScoreRatingType.Evaluator;
                    break;

                case (byte)ProcessType.Reviewer:
                    epaInfo.Process.CurrentProcess.MaxPerformanceScore = epaInfo.ScoreRatingType.Reviewer;
                    break;

                case (byte)ProcessType.Management:
                    epaInfo.Process.CurrentProcess.MaxPerformanceScore = epaInfo.ScoreRatingType.Management;
                    break;

                default:
                    epaInfo.Process.CurrentProcess.MaxPerformanceScore = 0;
                    break;
            }

            epaInfo.Process.Self.MaxPerformanceScore = epaInfo.ScoreRatingType.SelfEvaluated;

            if (epaInfo.HasDisciplineScore)
            {
                epaInfo.Process.Self.MaxPerformanceScore += epaInfo.MaxDisciplineScore;
                epaInfo.Process.CurrentProcess.MaxPerformanceScore += epaInfo.MaxDisciplineScore;
            }
        }

        public EPAProcessModel CloneEPAProcess(EPAProcessModel process)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, process);
                ms.Position = 0;

#pragma warning disable S5773 // Types allowed to be deserialized should be restricted
                return (EPAProcessModel)formatter.Deserialize(ms);
#pragma warning restore S5773 // Types allowed to be deserialized should be restricted
            }
        }

        public void SpreadKPIScore(IList<EPAKpiGroupModel> kpiGroups)
        {
            IUtilityService utilityService = new UtilityService();
            foreach (EPAKpiGroupModel kpiGroup in kpiGroups)
            {
                if (!kpiGroup.HasKPIOptional && kpiGroup.IsSpreadKPI && kpiGroup.Weight != null)
                {
                    double averageWeight = kpiGroup.Weight.Value / kpiGroup.KPIs.Count;
                    averageWeight = utilityService.SplitNumber(averageWeight, Constants.NumberOfDecimals);

                    foreach (EPAKpiModel kpi in kpiGroup.KPIs)
                    {
                        kpi.Weight = averageWeight;
                    }
                }
            }
        }

        public void SetMaxSubtractAndMaxAdditionScore(EPAModel epaInfo)
        {
            switch (epaInfo.Process.CurrentProcess.ProcessId)
            {
                case (byte)ProcessType.Reviewer:
                    CalculateEpaInfoForReviewer(epaInfo);
                    break;

                case (byte)ProcessType.Management:
                    CalculateEpaInfoForManagement(epaInfo);
                    break;

                default:
                    return;
            }
        }

        private static bool IsSA_HRRole(IList<Role> roles)
            => roles.Any(x => x.RoleId == (byte)SystemRole.HumanResource || x.RoleId == (byte)SystemRole.SystemAdmin);

        private static IList<EPAProcessModel> RemoveProcessOfOtherActor(IList<EPAProcessModel> processes, int currentEmployeeId)
            => processes.Where(x => x.EvaluatorInfo.EmployeeId != currentEmployeeId && x.StatusId != (byte)ProcessStatusType.Closed).ToList();

        private static void CalculateFinalScore(EPAModel epaInfo)
        {
            // Discipline Score
            float disciplineScore = 0;
            if (epaInfo.HasDisciplineScore)
            {
                disciplineScore = epaInfo.DisciplineScore;
            }

            var processes = epaInfo.AllProcesses
                .Where(x => x.StatusId > (byte)ProcessStatusType.Open && x.ProcessId > (byte)ProcessType.Self && !string.IsNullOrWhiteSpace(x.TotalScore));
            var maxProcessId = (byte)ProcessType.None;

            if (processes.Any())
            {
                maxProcessId = processes.Max(x => x.ProcessId);
            }

            switch (maxProcessId)
            {
                case (byte)ProcessType.Management:
                    epaInfo.FinalScoreToExport = epaInfo.Process.AverageScoreByManagements.HasValue
                        ? GetAverageTotalScore(epaInfo.Process.AverageScoreByManagements, disciplineScore)
                        : GetAverageTotalScore(epaInfo.Process.AverageScoreByReviewers, disciplineScore);
                    break;

                case (byte)ProcessType.Reviewer:
                    epaInfo.FinalScoreToExport = epaInfo.Process.AverageScoreByReviewers.HasValue
                        ? GetAverageTotalScore(epaInfo.Process.AverageScoreByReviewers, disciplineScore)
                        : GetAverageTotalScore(epaInfo.Process.AverageScoreByEvaluators, disciplineScore);
                    break;

                case (byte)ProcessType.Evaluator:
                    epaInfo.FinalScoreToExport = GetAverageTotalScore(epaInfo.Process.AverageScoreByEvaluators, disciplineScore);
                    break;

                default:
                    epaInfo.FinalScoreToExport = Lang.NotAvailable;
                    break;
            }
        }

        private static void CalculateFinalScoreForSemiHasMonthly(EPAModel epaInfo)
        {
            // Discipline Score
            float disciplineScore = 0;
            if (epaInfo.HasDisciplineScore)
            {
                disciplineScore = epaInfo.DisciplineScore;
            }

            var processes = epaInfo.AllProcesses
                .Where(x => x.StatusId > (byte)ProcessStatusType.Open && x.ProcessId > (byte)ProcessType.Evaluator);
            var maxProcessId = (byte)ProcessType.None;

            if (processes.Any())
            {
                maxProcessId = processes.Max(x => x.ProcessId);
            }

            switch (maxProcessId)
            {
                case (byte)ProcessType.Management:
                    epaInfo.FinalScoreToExport = epaInfo.Process.AverageScoreByManagements.HasValue
                        ? GetAverageTotalScore(epaInfo.Process.AverageScoreByManagements, disciplineScore)
                        : GetAverageTotalScore(epaInfo.Process.AverageScoreByReviewers, disciplineScore);
                    break;

                case (byte)ProcessType.Reviewer:
                    epaInfo.FinalScoreToExport = epaInfo.Process.AverageScoreByReviewers.HasValue
                        ? GetAverageTotalScore(epaInfo.Process.AverageScoreByReviewers, disciplineScore)
                        : GetAverageTotalScore(epaInfo.MonthlyEPA.AverageReviewerScoreConverted, disciplineScore);
                    break;

                default:
                    epaInfo.FinalScoreToExport = Lang.NotAvailable;
                    break;
            }
        }

        private static string GetAverageTotalScore(float? averageScore, float disciplineScore)
        {
            if (averageScore.HasValue)
            {
                var result = averageScore.Value + disciplineScore;

                return Math.Round((decimal)result, Constants.NumberOfDecimalDefault).ToString(CultureInfo.InvariantCulture);
            }

            return Lang.NotAvailable;
        }

        private static void RemoveSensitiveTotalScoreAndComment(IEnumerable<EPAProcessModel> processOfOtherActors)
        {
            if (processOfOtherActors.Any())
            {
                foreach (EPAProcessModel process in processOfOtherActors)
                {
                    process.TotalScore = string.Empty;
                    process.Comment = string.Empty;
                }
            }
        }

        private static void CalculateEpaInfoForManagement(EPAModel epaInfo)
        {
            float maxAdditionScore;
            float score;

            if (epaInfo.Process.Reviewers.Any())
            {
                score = -epaInfo.Process.AverageScoreByReviewers ?? 0;
                epaInfo.Process.CurrentProcess.MaxSubtractScore = score;
                maxAdditionScore = epaInfo.Process.CurrentProcess.MaxTotalScore + score;
            }
            else
            {
                if (epaInfo.IsSemiHasMonthly)
                {
                    score = -epaInfo.MonthlyEPA.AverageEvaluatorScoreConverted ?? 0;
                    epaInfo.Process.CurrentProcess.MaxSubtractScore = score;
                    maxAdditionScore = epaInfo.Process.CurrentProcess.MaxTotalScore + score;
                }
                else
                {
                    score = -epaInfo.Process.AverageScoreByEvaluators ?? 0;
                    epaInfo.Process.CurrentProcess.MaxSubtractScore = score;
                    maxAdditionScore = epaInfo.Process.CurrentProcess.MaxTotalScore + score;
                }
            }

            epaInfo.Process.CurrentProcess.MaxAdditionScore = (float)Math.Round(maxAdditionScore, Constants.NumberOfDecimalDefault);
        }

        private static void CalculateEpaInfoForReviewer(EPAModel epaInfo)
        {
            float maxAdditionScore;
            float score;

            if (epaInfo.IsSemiHasMonthly)
            {
                score = -epaInfo.MonthlyEPA.AverageReviewerScoreConverted ?? 0;
                epaInfo.Process.CurrentProcess.MaxSubtractScore = score;
                maxAdditionScore = epaInfo.Process.CurrentProcess.MaxTotalScore + score;
            }
            else
            {
                score = -epaInfo.Process.AverageScoreByEvaluators ?? 0;
                epaInfo.Process.CurrentProcess.MaxSubtractScore = score;
                maxAdditionScore = epaInfo.Process.CurrentProcess.MaxTotalScore + score;
            }

            epaInfo.Process.CurrentProcess.MaxAdditionScore = (float)Math.Round(maxAdditionScore, Constants.NumberOfDecimalDefault);
        }

        private static void FillContentRow(IList<EPAModel> epaInfos, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (EPAModel epaInfo in epaInfos)
            {
                FillRow(epaInfo, ref writer, ref rowIndex);
            }
        }

        private static void FillRow(EPAModel epaInfo, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            const int writeRowIndex = 3;

            object record = CreateRecord(epaInfo, ref rowIndex);
            writer.WriteRow(writeRowIndex, record);
        }

        private static object CreateRecord(EPAModel epaInfo, ref int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                epaInfo.Employee.Code,
                Nick = epaInfo.Employee.NickName,
                epaInfo.Employee.FullName,
                Section = epaInfo.Employee.GroupName,
                JobTitle = epaInfo.Employee.JobTitleName,
                epaInfo.OldFinalScore,
                NewFinalScore = epaInfo.FinalScore
            };
            rowIndex++;

            return record;
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
    }
}