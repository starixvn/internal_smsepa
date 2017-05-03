namespace SMS.EPA.Repositories
{
    using Fanex.Data.Repository;
    using Model;
    using System.Collections.Generic;

    public interface IEmployeeEPARepository
    {
        IEnumerable<EPAModel> GetEPA(ICriteria criteria);

        EPAModel GetEPAInfo(ICriteria criteria);

        IEnumerable<EPAModel> GetEPAEndEvaluationPeriod(ICriteria criteria);

        IEnumerable<TemporaryKPIModel> GetKPIByTargetTemplate(ICriteria criteria);

        IEnumerable<TemporaryKPIModel> GetKPIToView(ICriteria criteria);

        IEnumerable<RatingByKPIModel> GetEmployeeEPAByKPI(ICriteria criteria);

        DashboardModel GetDashboard(ICriteria criteria);

        IList<EPAModel> GetEPAToUpdateFinalScore(ICriteria criteria);

        EPARecommendationModel GetRecommendationById(ICriteria criteria);
    }
}