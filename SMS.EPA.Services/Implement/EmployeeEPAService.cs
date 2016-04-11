// -----------------------------------------------------------------------
// <copyright file="MemberEPAService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.04">Created</change>
// </history>

namespace SMS.EPA.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Core.Utilities;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Criteria.EmployeeEPA.RatingByKPI;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.EmployeeEPA.RatingByKPI;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.ScoreRatingType;
    using SMS.EPA.Model.TargetTemplate;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;

    public class EmployeeEPAService : BaseService, IEmployeeEPAService
    {
        private readonly IEmployeeEPARepository _employeeEPARepository;

        public EmployeeEPAService()
            : this(null)
        {
        }

        public EmployeeEPAService(IEmployeeEPARepository employeeEPARepository)
        {
            _employeeEPARepository = employeeEPARepository ?? new EmployeeEPARepository();
        }

        #region Self

        public IEnumerable<EPAModel> GetEPAByEmployee(GetEPAByEmployeeCriteria criteria)
        {
            IEnumerable<EPAModel> epas = new List<EPAModel>();

            if (criteria != null && criteria.IsValid())
            {
                epas = _employeeEPARepository.GetEPA(criteria);

                foreach (EPAModel epa in epas)
                {
                    epa.Process.CurrentProcess = epa.Process.Self;
                }
            }

            return epas;
        }

        public EPAModel GetEPAInfoByEmployee(GetEPAInfoByEmployeeCriteria criteria)
        {
            EPAModel epaInfo = new EPAModel();

            if (criteria != null && criteria.IsValid())
            {
                epaInfo = _employeeEPARepository.GetEPAInfo(criteria);
                epaInfo.Process.CurrentProcess = epaInfo.Process.Self;
                SetMaxPerformanceScore(epaInfo);
                SpreadKPIScore(epaInfo.KPIGroups);

                foreach (EPAKPIModel kpi in epaInfo.KPIGroups.SelectMany(x => x.KPIs))
                {
                    kpi.CurrentEvaluation = kpi.EvaluationBySelf;
                }

                if (epaInfo.HasMonthlyEPA && epaInfo.Period.TargetTypeId == (int)TargetType.SemiAnnual)
                {
                    epaInfo.MonthlyEPA.MonthlyEPAs = GetMonthlyEPAOfSemi(epaInfo.EPAId.ToString());
                    ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
                }
            }

            return epaInfo;
        }

        #endregion Self

        #region Evaluator

        public IEnumerable<EPAModel> GetEPAByEvaluator(GetEPAByEvaluatorCriteria criteria)
        {
            IEnumerable<EPAModel> memberEPAs = new List<EPAModel>();

            if (criteria != null && criteria.IsValid())
            {
                memberEPAs = _employeeEPARepository.GetEPA(criteria);

                IList<EPAModel> monthlyEPAs = new List<EPAModel>();
                string epaIds = string.Join(",", memberEPAs.Where(x => x.HasMonthlyEPA && x.Period.TargetTypeId == (int)TargetType.SemiAnnual).Select(x => x.EPAId));

                if (!string.IsNullOrEmpty(epaIds))
                {
                    monthlyEPAs = GetMonthlyEPAOfSemi(epaIds);
                }

                foreach (EPAModel memberEPA in memberEPAs)
                {
                    memberEPA.Process.CurrentProcess = memberEPA.Process.Evaluators.FirstOrDefault(x => x.EvaluatorId == criteria.EvaluatorId);
                    SetMaxPerformanceScore(memberEPA);
                    GroupMonthlyEPABySemi(memberEPA, monthlyEPAs);
                    ConvertAverageMonthlyScore(memberEPA.MonthlyEPA, memberEPA.ScoreRatingType);
                }
            }

            return memberEPAs;
        }

        public EPAModel GetEPAInfoByEvaluator(GetEPAInfoByEvaluatorCriteria criteria)
        {
            EPAModel epaInfo = new EPAModel();

            if (criteria != null && criteria.IsValid())
            {
                epaInfo = _employeeEPARepository.GetEPAInfo(criteria);
                epaInfo.Process.CurrentProcess = epaInfo.Process.Evaluators.FirstOrDefault(x => x.EvaluatorId == criteria.EvaluatorId);
                SetMaxPerformanceScore(epaInfo);
                SpreadKPIScore(epaInfo.KPIGroups);

                foreach (EPAKPIModel kpi in epaInfo.KPIGroups.SelectMany(x => x.KPIs))
                {
                    kpi.CurrentEvaluation = kpi.EvaluationByEvaluators.FirstOrDefault();
                }

                if (epaInfo.HasMonthlyEPA && epaInfo.Period.TargetTypeId == (int)TargetType.SemiAnnual)
                {
                    epaInfo.MonthlyEPA.MonthlyEPAs = GetMonthlyEPAOfSemi(epaInfo.EPAId.ToString());
                    ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
                }
            }

            return epaInfo;
        }

        #endregion Evaluator

        #region Reviewer

        public IEnumerable<EPAModel> GetEPAByReviewer(GetEPAByReviewerCriteria criteria)
        {
            IEnumerable<EPAModel> memberEPAs = new List<EPAModel>();

            if (criteria != null && criteria.IsValid())
            {
                memberEPAs = _employeeEPARepository.GetEPA(criteria);

                IList<EPAModel> monthlyEPAs = new List<EPAModel>();
                string epaIds = string.Join(",", memberEPAs.Where(x => x.HasMonthlyEPA && x.Period.TargetTypeId == (int)TargetType.SemiAnnual).Select(x => x.EPAId));

                if (!string.IsNullOrEmpty(epaIds))
                {
                    monthlyEPAs = GetMonthlyEPAOfSemi(epaIds);
                }

                foreach (EPAModel memberEPA in memberEPAs)
                {
                    memberEPA.Process.CurrentProcess = memberEPA.Process.Reviewers.FirstOrDefault(x => x.EvaluatorId == criteria.ReviewerId);
                    SetMaxPerformanceScore(memberEPA);
                    GroupMonthlyEPABySemi(memberEPA, monthlyEPAs);
                    ConvertAverageMonthlyScore(memberEPA.MonthlyEPA, memberEPA.ScoreRatingType);
                    SetMaxSubtractAndMaxAdditionScore(memberEPA);
                }
            }

            return memberEPAs;
        }

        public EPAModel GetEPAInfoByReviewer(GetEPAInfoByReviewerCriteria criteria)
        {
            EPAModel epaInfo = new EPAModel();

            if (criteria != null && criteria.IsValid())
            {
                epaInfo = _employeeEPARepository.GetEPAInfo(criteria);
                epaInfo.Process.CurrentProcess = epaInfo.Process.Reviewers.FirstOrDefault(x => x.EvaluatorId == criteria.ReviewerId);
                SetMaxPerformanceScore(epaInfo);
                SpreadKPIScore(epaInfo.KPIGroups);

                if (epaInfo.HasMonthlyEPA && epaInfo.Period.TargetTypeId == (int)TargetType.SemiAnnual)
                {
                    epaInfo.MonthlyEPA.MonthlyEPAs = GetMonthlyEPAOfSemi(epaInfo.EPAId.ToString());
                    ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
                }
            }

            return epaInfo;
        }

        #endregion Reviewer

        #region Management

        public IEnumerable<EPAModel> GetEPAByManagement(GetEPAByManagementCriteria criteria)
        {
            IEnumerable<EPAModel> memberEPAs = new List<EPAModel>();

            if (criteria != null && criteria.IsValid())
            {
                memberEPAs = _employeeEPARepository.GetEPA(criteria);

                IList<EPAModel> monthlyEPAs = new List<EPAModel>();
                string epaIds = string.Join(",", memberEPAs.Where(x => x.HasMonthlyEPA && x.Period.TargetTypeId == (int)TargetType.SemiAnnual).Select(x => x.EPAId));

                if (!string.IsNullOrEmpty(epaIds))
                {
                    monthlyEPAs = GetMonthlyEPAOfSemi(epaIds);
                }

                foreach (EPAModel memberEPA in memberEPAs)
                {
                    memberEPA.Process.CurrentProcess = memberEPA.Process.Managements.FirstOrDefault(x => x.EvaluatorId == criteria.ManagementId);
                    SetMaxPerformanceScore(memberEPA);
                    GroupMonthlyEPABySemi(memberEPA, monthlyEPAs);
                    ConvertAverageMonthlyScore(memberEPA.MonthlyEPA, memberEPA.ScoreRatingType);
                    SetMaxSubtractAndMaxAdditionScore(memberEPA);
                }
            }

            return memberEPAs;
        }

        public EPAModel GetEPAInfoByManagement(GetEPAInfoByManagementCriteria criteria)
        {
            EPAModel epaInfo = new EPAModel();

            if (criteria != null && criteria.IsValid())
            {
                epaInfo = _employeeEPARepository.GetEPAInfo(criteria);
                epaInfo.Process.CurrentProcess = epaInfo.Process.Managements.FirstOrDefault(x => x.EvaluatorId == criteria.ManagementId);
                SetMaxPerformanceScore(epaInfo);
                SpreadKPIScore(epaInfo.KPIGroups);

                if (epaInfo.HasMonthlyEPA && epaInfo.Period.TargetTypeId == (int)TargetType.SemiAnnual)
                {
                    epaInfo.MonthlyEPA.MonthlyEPAs = GetMonthlyEPAOfSemi(epaInfo.EPAId.ToString());
                    ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
                }
            }

            return epaInfo;
        }

        #endregion Management

        #region Rating By KPI

        public IEnumerable<TargetTemplateModel> GetTargetTemplateByEvaluator(GetTargetTemplateByEvaluatorCriteria criteria)
        {
            IEnumerable<TargetTemplateModel> targetTemplates = new List<TargetTemplateModel>();

            if (criteria != null && criteria.IsValid())
            {
                targetTemplates = GetData<TargetTemplateModel>(criteria);
            }

            return targetTemplates;
        }

        public IEnumerable<TargetTemplateModel> GetTargetTemplateToView(GetTargetTemplateToViewCriteria criteria)
        {
            IEnumerable<TargetTemplateModel> targetTemplates = new List<TargetTemplateModel>();

            if (criteria != null && criteria.IsValid())
            {
                targetTemplates = GetData<TargetTemplateModel>(criteria);
            }

            return targetTemplates;
        }

        public IEnumerable<TemporaryKPIModel> GetKPIByTargetTemplate(GetKPIByTargetTemplateCriteria criteria)
        {
            IEnumerable<TemporaryKPIModel> kpis = new List<TemporaryKPIModel>();

            if (criteria != null && criteria.IsValid())
            {
                kpis = _employeeEPARepository.GetKPIByTargetTemplate(criteria);
            }

            return kpis;
        }

        public IEnumerable<TemporaryKPIModel> GetKPIToView(GetKPIToViewCriteria criteria)
        {
            IEnumerable<TemporaryKPIModel> kpis = new List<TemporaryKPIModel>();

            if (criteria != null && criteria.IsValid())
            {
                kpis = _employeeEPARepository.GetKPIToView(criteria);
            }

            return kpis;
        }

        public IEnumerable<RatingByKPIModel> GetEmployeeEPAToView(GetEmployeeEPAToViewCriteria criteria)
        {
            IEnumerable<RatingByKPIModel> epas = new List<RatingByKPIModel>();

            if (criteria != null && criteria.IsValid())
            {
                epas = _employeeEPARepository.GetEmployeeEPAToView(criteria);
            }

            return epas;
        }

        public IEnumerable<RatingByKPIModel> GetEmployeeEPAByKPI(GetEmployeeEPAByKPICriteria criteria)
        {
            IEnumerable<RatingByKPIModel> epas = new List<RatingByKPIModel>();

            if (criteria != null && criteria.IsValid())
            {
                epas = _employeeEPARepository.GetEmployeeEPAByKPI(criteria);
            }

            return epas;
        }

        public CommandResult UpdateKPIScore(UpdateKPIScoreCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public IEnumerable<Employee> GetEvaluatorsByTargetTemplate(GetEvaluatorsByTargetTemplateCriteria criteria)
        {
            IEnumerable<Employee> employees = new List<Employee>();

            if (criteria != null && criteria.IsValid())
            {
                employees = GetData<Employee>(criteria);
            }

            return employees;
        }

        #endregion Rating By KPI

        public EPAModel GetEPAInfoById(GetEPAInfoByIdCriteria criteria)
        {
            EPAModel epaInfo = new EPAModel();

            if (criteria != null && criteria.IsValid())
            {
                epaInfo = _employeeEPARepository.GetEPAInfo(criteria);
                SpreadKPIScore(epaInfo.KPIGroups);

                if (epaInfo.HasMonthlyEPA && epaInfo.Period.TargetTypeId == (int)TargetType.SemiAnnual)
                {
                    epaInfo.MonthlyEPA.MonthlyEPAs = GetMonthlyEPAOfSemi(epaInfo.EPAId.ToString());
                    ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
                }
            }

            return epaInfo;
        }

        public IEnumerable<EPAModel> GetEPAEndEvaluationPeriod(GetEPAEndEvaluationPeriodCriteria criteria)
        {
            IEnumerable<EPAModel> epaInfos = new List<EPAModel>();

            if (criteria != null && criteria.IsValid())
            {
                epaInfos = _employeeEPARepository.GetEPAEndEvaluationPeriod(criteria);
            }

            return epaInfos;
        }

        public CommandResult UpdateEPAScore(UpdateEPAScoreCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult AdjustScoreByReviewerOrManagement(AdjustScoreByReviewerOrManagementCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
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

        public IEnumerable<StandardGradeModel> GetStandardGrade(GetStandardGradeCriteria criteria)
        {
            IEnumerable<StandardGradeModel> standardGrades = new List<StandardGradeModel>();

            if (criteria != null && criteria.IsValid())
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

            monthlyEPAs = _employeeEPARepository.GetEPA(criteria).ToList();

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

        private void GroupMonthlyEPABySemi(EPAModel semiEPA, IList<EPAModel> monthlyEPAs)
        {
            if (semiEPA != null && monthlyEPAs != null && monthlyEPAs.Count > 0)
            {
                foreach (EPAModel monthlyEPA in monthlyEPAs.Where(x => x.EPAParentId.Equals(semiEPA.EPAId)))
                {
                    semiEPA.MonthlyEPA.MonthlyEPAs.Add(monthlyEPA);
                }
            }
        }

        private void ConvertAverageMonthlyScore(MonthlyEPAModel monthlyEPA, ScoreRatingTypeModel scoreRatingType)
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

        private void SpreadKPIScore(IList<EPAKPIGroupModel> kpiGroups)
        {
            foreach (EPAKPIGroupModel kpiGroup in kpiGroups)
            {
                if (!kpiGroup.HasKPIOptional && kpiGroup.IsSpreadKPI && kpiGroup.Weight != null)
                {
                    double averageWeight = kpiGroup.Weight.Value / kpiGroup.KPIs.Count;
                    averageWeight = SplitNumber(averageWeight, Constants.NumberOfDecimals);

                    foreach (EPAKPIModel kpi in kpiGroup.KPIs)
                    {
                        kpi.Weight = averageWeight;
                    }
                }
            }
        }

        private double SplitNumber(double number, int numberOfDecimals)
        {
            double result = number;
            numberOfDecimals = numberOfDecimals <= 0 ? Constants.NumberOfDecimalDefault : numberOfDecimals;

            if ((number % 1) != 0)
            {
                int index = number.ToString().IndexOf('.');
                string naturalFraction = number.ToString().Substring(0, index),
                    decimalFraction = number.ToString().Substring(index + 1);

                result = double.Parse(naturalFraction + "." + decimalFraction.Substring(0, Math.Min(decimalFraction.Length, numberOfDecimals)));
            }

            return result;
        }

        private void SetMaxPerformanceScore(EPAModel epaInfo)
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
                epaInfo.Process.Self.MaxPerformanceScore += epaInfo.DisciplineScore;
                epaInfo.Process.CurrentProcess.MaxPerformanceScore += epaInfo.DisciplineScore;
            }
        }

        private void SetMaxSubtractAndMaxAdditionScore(EPAModel epaInfo)
        {
            float maxAdditionScore = 0;

            switch (epaInfo.Process.CurrentProcess.ProcessId)
            {
                case (int)ProcessType.Reviewer:
                    if (epaInfo.HasMonthlyEPA && epaInfo.Period.TargetTypeId == (int)TargetType.SemiAnnual)
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
                        if (epaInfo.HasMonthlyEPA && epaInfo.Period.TargetTypeId == (int)TargetType.SemiAnnual)
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
    }
}