namespace SMS.EPA.Services.Interfaces
{
    using SMS.EPA.Model;

    public interface IExportRecommendationReportService
    {
        void Export(EPASummaryRecommendationReportModel recommendations, string templatePath);
    }
}