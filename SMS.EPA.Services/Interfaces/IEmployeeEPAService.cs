namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Criteria.EmployeeEPA.RatingByKPI;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.EmployeeEPA.RatingByKPI;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.ScoreRatingType;
    using SMS.EPA.Model.TargetTemplate;

    public interface IEmployeeEPAService
    {
        #region Self

        IEnumerable<EPAModel> GetEPAByEmployee(GetEPAByEmployeeCriteria criteria);

        EPAModel GetEPAInfoByEmployee(GetEPAInfoByEmployeeCriteria criteria);

        #endregion Self

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

        IEnumerable<TargetTemplateModel> GetTargetTemplateByEvaluator(GetTargetTemplateByEvaluatorCriteria criteria);

        IEnumerable<TargetTemplateModel> GetTargetTemplateToView(GetTargetTemplateToViewCriteria criteria);

        IEnumerable<TemporaryKPIModel> GetKPIByTargetTemplate(GetKPIByTargetTemplateCriteria criteria);

        IEnumerable<EPAPeriodModel> GetPeriodToView(GetPeriodToViewCriteria criteria);

        IEnumerable<TemporaryKPIModel> GetKPIToView(GetKPIToViewCriteria criteria);

        IEnumerable<RatingByKPIModel> GetEmployeeEPAByKPI(GetEmployeeEPAByKPICriteria criteria);

        IEnumerable<RatingByKPIModel> GetEmployeeEPAToView(GetEmployeeEPAToViewCriteria criteria);

        IEnumerable<Employee> GetEvaluatorsByTargetTemplate(GetEvaluatorsByTargetTemplateCriteria criteria);

        CommandResult UpdateKPIScore(UpdateKPIScoreCriteria criteria);

        #endregion Rating By KPI

        IEnumerable<EPAModel> GetEPAEndEvaluationPeriod(GetEPAEndEvaluationPeriodCriteria criteria);

        EPAModel GetEPAInfoById(GetEPAInfoByIdCriteria criteria);

        CommandResult UpdateEPAScore(UpdateEPAScoreCriteria criteria);

        CommandResult AdjustScoreByReviewerOrManagement(AdjustScoreByReviewerOrManagementCriteria criteria);

        IEnumerable<Employee> GetMemberByEvaluatorOrReviewerOrManagement(GetMemberByEvaluatorOrReviewerOrManagementCriteria criteria);

        IEnumerable<Group> GetGroupByEvaluatorOrReviewerOrManagement(GetGroupByEvaluatorOrReviewerOrManagementCriteria criteria);

        IEnumerable<StandardGradeModel> GetStandardGrade(GetStandardGradeCriteria criteria);

        IEnumerable<EPASummaryImportModel> GetMemberEPASummaryImport(GetEPASummaryImportCriteria criteria);

        CommandResult UpdateEPASummaryImport(UpdateEPASummaryImportCriteria criteria);

        string ConvertScoreToGrade(float score, IEnumerable<StandardGradeModel> standardGrades);

        int CheckEmployeePermission(GetEmployeePermissionCriteria criteria);

        IEnumerable<EmployeeOtherViolationDetailModel> GetOtherViolationDetailByEPAId(GetOtherViolationDetailByEPAIdCriteria criteria);

        IList<EPAModel> GetMonthlyEPAOfSemi(string epaIds);

        CommandResult ResetScore(ResetScoreCriteria criteria);

        int GetNumberOfRow(GetNumberOfRowCriteria criteria);

        void GroupMonthlyEPABySemi(EPAModel semiEPA, IList<EPAModel> monthlyEPAs);

        void ConvertAverageMonthlyScore(MonthlyEPAModel monthlyEPA, ScoreRatingTypeModel scoreRatingType);
        
        IEnumerable<EPAModel> GetEPAEndEvaluationByPeriodId(GetEPAEndEvaluationByPeriodIdCriteria criteria);

        CommandResult UpdateFinalScoreEPAEndEvaluation(UpdateFinalScoreEPAEndEvaluationCriteria criteria);

        IEnumerable<EPAEmployeeModel> GetEmployeeToUpdateFinalScore(GetEmployeeToUpdateFinalScoreCriteria criteria);
    }
}