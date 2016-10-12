namespace SMS.EPA.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Core.Utilities;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.ScoreRatingType;
    using SMS.EPA.Repositories;
    using SMS.EPA.Services.Interfaces;

    public class EmployeeEPAService : BaseService, IEmployeeEPAService
    {
        private readonly IEmployeeEPARepository employeeEPARepository;

        public EmployeeEPAService()
            : this(null)
        {
        }

        public EmployeeEPAService(IEmployeeEPARepository employeeEPARepository)
        {
            this.employeeEPARepository = employeeEPARepository ?? new EmployeeEPARepository();
        }

        public EPAModel GetEPAInfoById(int epaId)
        {
            var criteria = new GetEPAInfoByIdCriteria
            {
                EPAId = epaId
            };
            EPAModel epaInfo = new EPAModel();

            if (criteria.IsValid())
            {
                epaInfo = employeeEPARepository.GetEPAInfo(criteria);
                SpreadKPIScore(epaInfo.KPIGroups);

                if (epaInfo.IsSemiHasMonthly)
                {
                    epaInfo.MonthlyEPA.MonthlyEPAs = GetMonthlyEPAOfSemi(epaInfo.EPAId.ToString());
                    ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
                }

                if (epaInfo.IsProbation || epaInfo.IsPromotion || epaInfo.IsSemiAnnual)
                {
                    epaInfo.Recommendation = GetRecommendationByEPAId(epaInfo.EPAId);
                }

                IEnumerable<StandardGradeModel> standardGrades = GetStandardGrade();
                float finalScore = epaInfo.FinalScore.Length > 0 ? float.Parse(epaInfo.FinalScore) : 0;
                epaInfo.Grade = ConvertScoreToGrade(finalScore, standardGrades);
            }

            return epaInfo;
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

        public EPARecommendationModel GetRecommendationByEPAId(int epaId)
        {
            var criteria = new GetRecommendationByEPAIdCriteria
            {
                EPAId = epaId
            };
            var recommendation = new EPARecommendationModel();

            if (criteria.IsValid())
            {
                recommendation = employeeEPARepository.GetRecommendationById(criteria);
            }

            return recommendation;
        }

        public IEnumerable<Employee> GetMemberByEvaluatorOrReviewerOrManagement(GetMemberByEvaluatorOrReviewerOrManagementCriteria criteria)
        {
            IEnumerable<Employee> employees = new List<Employee>();

            if (criteria != null && criteria.IsValid())
            {
                employees = GetData<Employee>(criteria);
            }

            return employees;
        }

        public IEnumerable<Group> GetGroupByEvaluatorOrReviewerOrManagement(GetGroupByEvaluatorOrReviewerOrManagementCriteria criteria)
        {
            IEnumerable<Group> groups = new List<Group>();

            if (criteria != null && criteria.IsValid())
            {
                groups = GetData<Group>(criteria);
            }

            return groups;
        }

        public IEnumerable<StandardGradeModel> GetStandardGrade()
        {
            var criteria = new GetStandardGradeCriteria();
            IEnumerable<StandardGradeModel> standardGrades = new List<StandardGradeModel>();

            if (criteria.IsValid())
            {
                standardGrades = GetData<StandardGradeModel>(criteria);
            }

            return standardGrades;
        }

        public IEnumerable<EPASummaryImportModel> GetMemberEPASummaryImport(GetEPASummaryImportCriteria criteria)
        {
            IEnumerable<EPASummaryImportModel> memberEPAs = new List<EPASummaryImportModel>();

            if (criteria != null && criteria.IsValid())
            {
                memberEPAs = GetData<EPASummaryImportModel>(criteria);
            }

            return memberEPAs;
        }

        public CommandResult UpdateEPASummaryImport(UpdateEPASummaryImportCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public string ConvertScoreToGrade(float score, IEnumerable<StandardGradeModel> standardGrades)
        {
            string grade = string.Empty;

            if (standardGrades != null && standardGrades.Any())
            {
                foreach (StandardGrade gradeEnum in standardGrades.Where(x => x.LeftScore <= score && x.RightScore >= score).Select(x => (StandardGrade)x.GradeId))
                {
                    grade = Enumerations.GetDescription(gradeEnum);
                    break;
                }
            }

            return grade;
        }

        public int CheckEmployeePermission(GetEmployeePermissionCriteria criteria)
        {
            int result = -1;

            if (criteria != null && criteria.IsValid())
            {
                result = GetDataEntity<int>(criteria);
            }

            return result;
        }

        public IEnumerable<EmployeeOtherViolationDetailModel> GetOtherViolationDetailByEPAId(GetOtherViolationDetailByEPAIdCriteria criteria)
        {
            IEnumerable<EmployeeOtherViolationDetailModel> otherViolationDetails = new List<EmployeeOtherViolationDetailModel>();

            if (criteria != null && criteria.IsValid())
            {
                otherViolationDetails = GetData<EmployeeOtherViolationDetailModel>(criteria);
            }

            return otherViolationDetails;
        }

        public IList<EPAModel> GetMonthlyEPAOfSemi(string epaIds)
        {
            IList<EPAModel> monthlyEPAs = new List<EPAModel>();

            var criteria = new GetMonthlyEPAOfSemiCriteria()
            {
                EPAIds = epaIds
            };

            monthlyEPAs = employeeEPARepository.GetEPA(criteria).ToList();

            return monthlyEPAs;
        }

        public CommandResult ResetScore(ResetScoreCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public IList<EPAPeriodModel> GetPeriodToUpdateFinalScore(GetPeriodToUpdateFinalScoreCriteria criteria)
        {
            IList<EPAPeriodModel> periods = new List<EPAPeriodModel>();

            if (criteria != null && criteria.IsValid())
            {
                periods = GetData<EPAPeriodModel>(criteria).ToList();
            }

            return periods;
        }

        public IList<EPAEmployeeModel> GetEmployeeToUpdateFinalScore(GetEmployeeToUpdateFinalScoreCriteria criteria)
        {
            IList<EPAEmployeeModel> employees = new List<EPAEmployeeModel>();

            if (criteria != null && criteria.IsValid())
            {
                employees = GetData<EPAEmployeeModel>(criteria).ToList();
            }

            return employees;
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

        public CommandResult UpdateFinalScoreEPAEndEvaluation(UpdateFinalScoreEPAEndEvaluationCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
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
                ScoreRatingTypeModel monthlyScoreRatingType = monthlyEPA.MonthlyEPAs.First().ScoreRatingType;

                float averageScore = (monthlyEPA.AverageSelfScore * scoreRatingType.SelfEvaluated) / monthlyScoreRatingType.SelfEvaluated;
                monthlyEPA.AverageSelfScoreConverted = (float)Math.Round(averageScore, 2);

                averageScore = (monthlyEPA.AverageEvaluatorScores * scoreRatingType.Evaluator) / monthlyScoreRatingType.Evaluator;
                monthlyEPA.AverageEvaluatorScoreConverted = (float)Math.Round(averageScore, 2);

                averageScore = (monthlyEPA.AverageReviewerScores * scoreRatingType.Reviewer) / monthlyScoreRatingType.Reviewer;
                monthlyEPA.AverageReviewerScoreConverted = (float)Math.Round(averageScore, 2);
            }
        }

        public void ExportEPAAfterUpdateFinalScore(string periodName, IList<EPAModel> epaInfos, string templatePath)
        {
            if (epaInfos != null && epaInfos.Any())
            {
                var writer = new ExcelTemplateWriter(templatePath, "Update_Final_Score", "Update Final Score");

                // Fill report title.
                writer.WriteRow(0, new object[] { periodName });
                writer.CopyRows(1, 1);

                // Fill report header.
                writer.CopyRows(2, 1);

                FillContentRow(epaInfos, ref writer);
                writer.SyncColumnsWidth(9);
                writer.CreateFreezePane(0, 3);

                writer.Render();
            }
        }

        public void SetMaxPerformanceScore(EPAModel epaInfo)
        {
            switch (epaInfo.Process.CurrentProcess.ProcessId)
            {
                case (int)ProcessType.Self:
                    epaInfo.Process.CurrentProcess.MaxPerformanceScore = epaInfo.ScoreRatingType.SelfEvaluated;
                    break;

                case (int)ProcessType.Evaluator:
                    epaInfo.Process.CurrentProcess.MaxPerformanceScore = epaInfo.ScoreRatingType.Evaluator;
                    break;

                case (int)ProcessType.Reviewer:
                    epaInfo.Process.CurrentProcess.MaxPerformanceScore = epaInfo.ScoreRatingType.Reviewer;
                    break;

                case (int)ProcessType.Management:
                    epaInfo.Process.CurrentProcess.MaxPerformanceScore = epaInfo.ScoreRatingType.Management;
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
            using (MemoryStream ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, process);
                ms.Position = 0;

                return (EPAProcessModel)formatter.Deserialize(ms);
            }
        }

        public void SpreadKPIScore(IList<EPAKPIGroupModel> kpiGroups)
        {
            IUtilityService utilityService = new UtilityService();
            foreach (EPAKPIGroupModel kpiGroup in kpiGroups)
            {
                if (!kpiGroup.HasKPIOptional && kpiGroup.IsSpreadKPI && kpiGroup.Weight != null)
                {
                    double averageWeight = kpiGroup.Weight.Value / kpiGroup.KPIs.Count;
                    averageWeight = utilityService.SplitNumber(averageWeight, Constants.NumberOfDecimals);

                    foreach (EPAKPIModel kpi in kpiGroup.KPIs)
                    {
                        kpi.Weight = averageWeight;
                    }
                }
            }
        }

        public void SetMaxSubtractAndMaxAdditionScore(EPAModel epaInfo)
        {
            float maxAdditionScore = 0;

            switch (epaInfo.Process.CurrentProcess.ProcessId)
            {
                case (int)ProcessType.Reviewer:
                    if (epaInfo.IsSemiHasMonthly)
                    {
                        epaInfo.Process.CurrentProcess.MaxSubtractScore = -epaInfo.MonthlyEPA.AverageReviewerScoreConverted;
                        maxAdditionScore = epaInfo.Process.CurrentProcess.MaxTotalScore - epaInfo.MonthlyEPA.AverageReviewerScoreConverted;
                    }
                    else
                    {
                        epaInfo.Process.CurrentProcess.MaxSubtractScore = -epaInfo.Process.AverageScoreByEvaluators;
                        maxAdditionScore = epaInfo.Process.CurrentProcess.MaxTotalScore - epaInfo.Process.AverageScoreByEvaluators;
                    }

                    epaInfo.Process.CurrentProcess.MaxAdditionScore = (float)Math.Round(maxAdditionScore, Constants.NumberOfDecimalDefault);
                    break;

                case (int)ProcessType.Management:
                    if (epaInfo.Process.Reviewers.Count > 0)
                    {
                        epaInfo.Process.CurrentProcess.MaxSubtractScore = -epaInfo.Process.AverageScoreByReviewers;
                        maxAdditionScore = epaInfo.Process.CurrentProcess.MaxTotalScore - epaInfo.Process.AverageScoreByReviewers;
                    }
                    else
                    {
                        if (epaInfo.IsSemiHasMonthly)
                        {
                            epaInfo.Process.CurrentProcess.MaxSubtractScore = -epaInfo.MonthlyEPA.AverageEvaluatorScoreConverted;
                            maxAdditionScore = epaInfo.Process.CurrentProcess.MaxTotalScore - epaInfo.MonthlyEPA.AverageEvaluatorScoreConverted;
                        }
                        else
                        {
                            epaInfo.Process.CurrentProcess.MaxSubtractScore = -epaInfo.Process.AverageScoreByEvaluators;
                            maxAdditionScore = epaInfo.Process.CurrentProcess.MaxTotalScore - epaInfo.Process.AverageScoreByEvaluators;
                        }
                    }

                    epaInfo.Process.CurrentProcess.MaxAdditionScore = (float)Math.Round(maxAdditionScore, Constants.NumberOfDecimalDefault);
                    break;
            }
        }

        private void FillContentRow(IList<EPAModel> epaInfos, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (EPAModel epaInfo in epaInfos)
            {
                FillRow(epaInfo, ref writer, ref rowIndex);
            }
        }

        private void FillRow(EPAModel epaInfo, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecord(epaInfo, ref rowIndex);
            writer.WriteRow(3, record);
        }

        private object CreateRecord(EPAModel epaInfo, ref int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                Code = epaInfo.Employee.Code,
                Nick = epaInfo.Employee.NickName,
                FullName = epaInfo.Employee.FullName,
                Section = epaInfo.Employee.GroupName,
                JobTitle = epaInfo.Employee.JobTitleName,
                OldFinalScore = epaInfo.OldFinalScore,
                NewFinalScore = epaInfo.FinalScore
            };
            rowIndex++;

            return record;
        }
    }
}