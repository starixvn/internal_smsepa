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
                    epa.CurrentProcess = epa.ProcessBySelf;
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
                epaInfo.CurrentProcess = epaInfo.ProcessBySelf;

                foreach (EPAKPIGroupModel kpiGroup in epaInfo.KPIGroups)
                {
                    foreach (EPAKPIModel kpi in kpiGroup.KPIs)
                    {
                        kpi.CurrentEvaluation = kpi.EvaluationBySelf;
                    }
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
                    memberEPA.CurrentProcess = memberEPA.ProcessByEvaluators.FirstOrDefault();
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
                epaInfo.CurrentProcess = epaInfo.ProcessByEvaluators.FirstOrDefault();

                foreach (EPAKPIGroupModel kpiGroup in epaInfo.KPIGroups)
                {
                    foreach (EPAKPIModel kpi in kpiGroup.KPIs)
                    {
                        kpi.CurrentEvaluation = kpi.EvaluationByEvaluators.FirstOrDefault();
                    }
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
                    memberEPA.CurrentProcess = memberEPA.ProcessByReviewers.FirstOrDefault();
                    GroupMonthlyEPABySemi(memberEPA, monthlyEPAs);
                    ConvertAverageMonthlyScore(memberEPA.MonthlyEPA, memberEPA.ScoreRatingType);
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

                epaInfo.CurrentProcess = epaInfo.ProcessByReviewers.FirstOrDefault();

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
                    memberEPA.CurrentProcess = memberEPA.ProcessByManagements.FirstOrDefault();
                    GroupMonthlyEPABySemi(memberEPA, monthlyEPAs);
                    ConvertAverageMonthlyScore(memberEPA.MonthlyEPA, memberEPA.ScoreRatingType);
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

                epaInfo.CurrentProcess = epaInfo.ProcessByManagements.FirstOrDefault();

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

                if (epaInfo.HasMonthlyEPA && epaInfo.Period.TargetTypeId == (int)TargetType.SemiAnnual)
                {
                    epaInfo.MonthlyEPA.MonthlyEPAs = GetMonthlyEPAOfSemi(epaInfo.EPAId.ToString());
                    ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
                }
            }

            return epaInfo;
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
                foreach (StandardGradeModel standardGrade in standardGrades)
                {
                    if (score >= standardGrade.LeftScore && score <= standardGrade.RightScore)
                    {
                        StandardGrade gradeEnum = (StandardGrade)standardGrade.GradeId;
                        grade = Enumerations.GetDescription(gradeEnum);
                        break;
                    }
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
                foreach (EPAModel monthlyEPA in monthlyEPAs)
                {
                    if (monthlyEPA.EPAParentId.Equals(semiEPA.EPAId))
                    {
                        semiEPA.MonthlyEPA.MonthlyEPAs.Add(monthlyEPA);
                    }
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
    }
}