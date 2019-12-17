namespace SMS.EPA.Services.Interfaces
{
    using SMS.EPA.Model;

    public interface IRecomendationReportService
    {
        EPASummaryRecommendationReportModel GetRecommendationReport(int periodId);
    }
}