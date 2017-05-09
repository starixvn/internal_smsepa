namespace SMS.EPA.Services.Interfaces
{
    using Core.Model;
    using Criteria;
    using Model;
    using Model.Enitites;
    using System.Collections.Generic;

    public interface IEmployeeEPAService
    {
        IEnumerable<EPAModel> GetEPAEndEvaluationPeriod(GetEPAEndEvaluationPeriodCriteria criteria);

        EPAModel GetEPAInfoById(int epaId);

        EPARecommendationModel GetRecommendationByEPAId(int epaId);

        IEnumerable<Employee> GetMemberByEvaluatorOrReviewerOrManagement(GetMemberByEvaluatorOrReviewerOrManagementCriteria criteria);

        IEnumerable<Group> GetGroupByEvaluatorOrReviewerOrManagement(GetGroupByEvaluatorOrReviewerOrManagementCriteria criteria);

        IEnumerable<StandardGradeModel> GetStandardGrade();

        IEnumerable<EPASummaryImportModel> GetMemberEPASummaryImport();

        CommandResult UpdateEPASummaryImport(UpdateEPASummaryImportCriteria criteria);

        string ConvertScoreToGrade(float performanceScore, float disciplineScore, bool hasDisciplineScore, IEnumerable<StandardGradeModel> standardGrades);

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

        void SpreadKPIScore(IList<EPAKpiGroupModel> kpiGroups);

        void SetMaxSubtractAndMaxAdditionScore(EPAModel epaInfo);

        void ExportEPAAfterUpdateFinalScore(string periodName, IList<EPAModel> epaInfos, string templatePath);
    }
}