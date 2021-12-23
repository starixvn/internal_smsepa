namespace SMS.EPA.Services
{
    using System.Collections.Generic;
    using HRM.UserControl.Client.Core.Session;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;

    public interface IEmployeeEPAService
    {
        EPAProcessModel CloneEPAProcess(EPAProcessModel process);

        void ConvertAverageMonthlyScore(MonthlyEPAModel monthlyEPA, ScoreRatingTypeModel scoreRatingType);

        string ConvertScoreToGrade(float? performanceScore, float disciplineScore, bool hasDisciplineScore, IEnumerable<StandardGradeModel> standardGrades);

        void ExportEPAAfterUpdateFinalScore(string periodName, IList<EPAModel> epaInfos, string templatePath);

        IEnumerable<EPAModel> GetEPAEndEvaluationPeriod(GetEPAEndEvaluationPeriodCriteria criteria);

        EPAModel GetEPAInfoById(int epaId);

        void RemoveSensitiveDataOnEmployeeRole(EPAModel epaInfo, IUserControlSession userControlSession);

        IList<EPAModel> GetEPAToUpdateFinalScore(GetEPAToUpdateFinalScoreCriteria criteria);

        IList<EPAModel> GetMonthlyEPAOfSemi(string epaIds);

        IEnumerable<EPARecommendation> GetRecommendationByEPAId(int epaId);

        void GroupMonthlyEPABySemi(EPAModel semiEPA, IList<EPAModel> monthlyEPAs);

        void SetMaxPerformanceScore(EPAModel epaInfo);

        void SetMaxSubtractAndMaxAdditionScore(EPAModel epaInfo);

        void SpreadKPIScore(IList<EPAKpiGroupModel> kpiGroups);

        EPAProbationExtendedQuestionModel GetExtendedQuestionByEPAId(int epaId);

        CommandResult GetDataAndUpdateFinalScore(int modifiedBy, int periodId, string employeeIds = null);
    }
}