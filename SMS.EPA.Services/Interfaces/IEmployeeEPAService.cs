namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Core.Services;
    using Criteria;
    using Model;
    using Model.Enitites;

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

        void RemoveSensitiveDataOnEmployeeRole(EPAModel epaInfo, ISessionService sessionService);

        IList<EPAModel> GetEPAToUpdateFinalScore(GetEPAToUpdateFinalScoreCriteria criteria);

        IEnumerable<Group> GetGroupByEvaluatorOrReviewerOrManagement(GetGroupByEvaluatorOrReviewerOrManagementCriteria criteria);

        IEnumerable<Employee> GetMemberByEvaluatorOrReviewerOrManagement(GetMemberByEvaluatorOrReviewerOrManagementCriteria criteria);

        IEnumerable<EPASummaryImportModel> GetMemberEPASummaryImport();

        IList<EPAModel> GetMonthlyEPAOfSemi(string epaIds);

        IList<EPAPeriodModel> GetPeriodToUpdateFinalScore(GetPeriodToUpdateFinalScoreCriteria criteria);

        EPARecommendationModel GetRecommendationByEPAId(int epaId);

        IEnumerable<StandardGradeModel> GetStandardGrade();

        void GroupMonthlyEPABySemi(EPAModel semiEPA, IList<EPAModel> monthlyEPAs);

        void SetMaxPerformanceScore(EPAModel epaInfo);

        void SetMaxSubtractAndMaxAdditionScore(EPAModel epaInfo);

        void SpreadKPIScore(IList<EPAKpiGroupModel> kpiGroups);
    }
}