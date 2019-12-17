namespace SMS.EPA.Services.Implement
{
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.Report;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Interfaces;

    public class RecomendationReportService : IRecomendationReportService
    {
        private readonly IPeriodService periodService;
        private readonly IBaseService baseService;

        public RecomendationReportService() : this(null, null)
        {
        }

        public RecomendationReportService(IPeriodService periodService, IBaseService baseService)
        {
            this.periodService = periodService ?? new PeriodService();
            this.baseService = baseService ?? new BaseService();
        }

        public EPASummaryRecommendationReportModel GetRecommendationReport(int periodId)
        {
            var criteria = new GetEPARecommendationReportCriteria
            {
                PeriodId = periodId
            };

            var recommendationReport = new EPASummaryRecommendationReportModel
            {
                Recommendations = baseService.GetData<EPASummaryRecommendationModel>(criteria),
                Period = periodService.GetPeriodById(periodId)
            };

            return recommendationReport;
        }
    }
}