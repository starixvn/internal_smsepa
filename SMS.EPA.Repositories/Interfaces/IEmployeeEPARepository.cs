namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Model;

    public interface IEmployeeEPARepository
    {
        IEnumerable<EPAModel> GetEPA(ICriteria criteria);

        EPAModel GetEPAInfo(ICriteria criteria);

        IEnumerable<EPAModel> GetEPAEndEvaluationPeriod(ICriteria criteria);

        IEnumerable<TemporaryKPIModel> GetKPIByTargetTemplate(ICriteria criteria);

        IEnumerable<TemporaryKPIModel> GetKPIToView(ICriteria criteria);

        IEnumerable<RatingByKPIModel> GetEmployeeEPAByKPI(ICriteria criteria);

        IEnumerable<RatingByKPIModel> GetEmployeeEPAToView(ICriteria criteria);

        DashboardModel GetDashboard(ICriteria criteria);

        IList<EPAModel> GetEPAToUpdateFinalScore(ICriteria criteria);

        EPARecommendationModel GetRecommendationById(ICriteria criteria);
    }
}