namespace SMS.EPA.Services.Interfaces
{
    using Core.Model;
    using Criteria;
    using Model;
    using Model.Enitites;
    using System.Collections.Generic;

    public interface IEmployeeEPAService
    {
        int CheckEmployeePermission(GetEmployeePermissionCriteria criteria);

        EPAProcessModel CloneEPAProcess(EPAProcessModel process);

        void ConvertAverageMonthlyScore(MonthlyEPAModel monthlyEPA, ScoreRatingTypeModel scoreRatingType);

        string ConvertScoreToGrade(float? performanceScore, float disciplineScore, bool hasDisciplineScore, IEnumerable<StandardGradeModel> standardGrades);

        void ExportEPAAfterUpdateFinalScore(string periodName, IList<EPAModel> epaInfos, string templatePath);

        IList<EPAEmployeeModel> GetEmployeeToUpdateFinalScore(GetEmployeeToUpdateFinalScoreCriteria criteria);

        IEnumerable<EPAModel> GetEPAEndEvaluationPeriod(GetEPAEndEvaluationPeriodCriteria criteria);

        EPAModel GetEPAInfoById(int epaId);

        IList<EPAModel> GetEPAToUpdateFinalScore(GetEPAToUpdateFinalScoreCriteria criteria);

        IEnumerable<Group> GetGroupByEvaluatorOrReviewerOrManagement(GetGroupByEvaluatorOrReviewerOrManagementCriteria criteria);

        IEnumerable<Employee> GetMemberByEvaluatorOrReviewerOrManagement(GetMemberByEvaluatorOrReviewerOrManagementCriteria criteria);

        IEnumerable<EPASummaryImportModel> GetMemberEPASummaryImport();

        IList<EPAModel> GetMonthlyEPAOfSemi(string epaIds);

        IEnumerable<EmployeeOtherViolationDetailModel> GetOtherViolationDetailByEPAId(GetOtherViolationDetailByEPAIdCriteria criteria);

        IList<EPAPeriodModel> GetPeriodToUpdateFinalScore(GetPeriodToUpdateFinalScoreCriteria criteria);

        EPARecommendationModel GetRecommendationByEPAId(int epaId);

        IEnumerable<StandardGradeModel> GetStandardGrade();

        void GroupMonthlyEPABySemi(EPAModel semiEPA, IList<EPAModel> monthlyEPAs);

        CommandResult ResetScore(ResetScoreCriteria criteria);

        void SetMaxPerformanceScore(EPAModel epaInfo);

        void SetMaxSubtractAndMaxAdditionScore(EPAModel epaInfo);

        void SpreadKPIScore(IList<EPAKpiGroupModel> kpiGroups);

        CommandResult UpdateEPASummaryImport(UpdateEPASummaryImportCriteria criteria);

        CommandResult UpdateFinalScoreEPAEndEvaluation(UpdateFinalScoreEPAEndEvaluationCriteria criteria);
    }
}