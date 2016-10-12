namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.ScoreRatingType;

    public interface IEmployeeEPAService
    {
        #region Evaluator

        IEnumerable<EPAModel> GetEPAByEvaluator(GetEPAByEvaluatorCriteria criteria);

        EPAModel GetEPAInfoByEvaluator(GetEPAInfoByEvaluatorCriteria criteria);

        #endregion Evaluator

        #region Reviewer

        IEnumerable<EPAModel> GetEPAByReviewer(GetEPAByReviewerCriteria criteria);

        EPAModel GetEPAInfoByReviewer(GetEPAInfoByReviewerCriteria criteria);

        #endregion Reviewer

        #region Management

        IEnumerable<EPAModel> GetEPAByManagement(GetEPAByManagementCriteria criteria);

        EPAModel GetEPAInfoByManagement(GetEPAInfoByManagementCriteria criteria);

        #endregion Management

        #region Rating By KPI

        IEnumerable<EPAPeriodModel> GetPeriodByEvaluator(GetPeriodByEvaluatorCriteria criteria);

        IEnumerable<TargetTemplate> GetTargetTemplateByEvaluator(GetTargetTemplateByEvaluatorCriteria criteria);

        IEnumerable<TargetTemplate> GetTargetTemplateToView(GetTargetTemplateToViewCriteria criteria);

        IEnumerable<TemporaryKPIModel> GetKPIByTargetTemplate(GetKPIByTargetTemplateCriteria criteria);

        IEnumerable<EPAPeriodModel> GetPeriodToView(GetPeriodToViewCriteria criteria);

        IEnumerable<TemporaryKPIModel> GetKPIToView(GetKPIToViewCriteria criteria);

        IEnumerable<RatingByKPIModel> GetEmployeeEPAByKPI(GetEmployeeEPAByKPICriteria criteria);

        IEnumerable<RatingByKPIModel> GetEmployeeEPAToView(GetEmployeeEPAToViewCriteria criteria);

        IEnumerable<Employee> GetEvaluatorsByTargetTemplate(GetEvaluatorsByTargetTemplateCriteria criteria);

        CommandResult UpdateKPIScore(UpdateKPIScoreCriteria criteria);

        #endregion Rating By KPI

        IEnumerable<EPAModel> GetEPAEndEvaluationPeriod(GetEPAEndEvaluationPeriodCriteria criteria);

        EPAModel GetEPAInfoById(int epaId);

        EPARecommendationModel GetRecommendationByEPAId(int epaId);

        CommandResult UpdateEPAScore(UpdateEPAScoreCriteria criteria);

        CommandResult AdjustScoreByReviewerOrManagement(AdjustScoreByReviewerOrManagementCriteria criteria);

        IEnumerable<Employee> GetMemberByEvaluatorOrReviewerOrManagement(GetMemberByEvaluatorOrReviewerOrManagementCriteria criteria);

        IEnumerable<Group> GetGroupByEvaluatorOrReviewerOrManagement(GetGroupByEvaluatorOrReviewerOrManagementCriteria criteria);

        IEnumerable<StandardGradeModel> GetStandardGrade();

        IEnumerable<EPASummaryImportModel> GetMemberEPASummaryImport(GetEPASummaryImportCriteria criteria);

        CommandResult UpdateEPASummaryImport(UpdateEPASummaryImportCriteria criteria);

        string ConvertScoreToGrade(float score, IEnumerable<StandardGradeModel> standardGrades);

        int CheckEmployeePermission(GetEmployeePermissionCriteria criteria);

        IEnumerable<EmployeeOtherViolationDetailModel> GetOtherViolationDetailByEPAId(GetOtherViolationDetailByEPAIdCriteria criteria);

        IList<EPAModel> GetMonthlyEPAOfSemi(string epaIds);

        CommandResult ResetScore(ResetScoreCriteria criteria);

        void GroupMonthlyEPABySemi(EPAModel semiEPA, IList<EPAModel> monthlyEPAs);

        void ConvertAverageMonthlyScore(MonthlyEPAModel monthlyEPA, ScoreRatingTypeModel scoreRatingType);

        CommandResult UpdateFinalScoreEPAEndEvaluation(UpdateFinalScoreEPAEndEvaluationCriteria criteria);

        IList<EPAEmployeeModel> GetEmployeeToUpdateFinalScore(GetEmployeeToUpdateFinalScoreCriteria criteria);

        IList<EPAPeriodModel> GetPeriodToUpdateFinalScore(GetPeriodToUpdateFinalScoreCriteria criteria);

        IList<EPAModel> GetEPAToUpdateFinalScore(GetEPAToUpdateFinalScoreCriteria criteria);

        void SetMaxPerformanceScore(EPAModel epaInfo);

        EPAProcessModel CloneEPAProcess(EPAProcessModel process);

        void SpreadKPIScore(IList<EPAKPIGroupModel> kpiGroups);

        void ExportEPAAfterUpdateFinalScore(string periodName, IList<EPAModel> epaInfos, string templatePath);
    }
}