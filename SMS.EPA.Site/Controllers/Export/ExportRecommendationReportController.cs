namespace SMS.EPA.Site.Controllers
{
    using System.Web.Mvc;
    using Model;
    using Services.Implement;
    using Services.Interfaces;

    public class ExportRecommendationReportController : Controller
    {
        private const string RecommendationReportTemplate = @"~\ExcelTemplates\RecommendationReportTemplate.xls";
        private readonly IRecomendationReportService _recomendationReportService;
        private readonly IExportRecommendationReportService _exportExcelService;

        public ExportRecommendationReportController()
            : this(null, null)
        {
        }

        public ExportRecommendationReportController(
            IRecomendationReportService recomendationReportService,
            IExportRecommendationReportService exportExcelService
            )
        {
            _recomendationReportService = recomendationReportService ?? new RecomendationReportService();
            _exportExcelService = exportExcelService ?? new ExportRecommendationReportService();
        }

        public ActionResult ExportExcel(int periodId)
        {
            EPASummaryRecommendationReportModel data = _recomendationReportService.GetRecommendationReport(periodId);

            _exportExcelService.Export(data, Server.MapPath(RecommendationReportTemplate));

            return new EmptyResult();
        }
    }
}