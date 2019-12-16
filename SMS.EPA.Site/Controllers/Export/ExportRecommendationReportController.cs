namespace SMS.EPA.Site.Controllers
{
    using System.Web.Mvc;
    using Criteria.Report;
    using Model;
    using Services.Implement;
    using Services.Interfaces;

    public class ExportRecommendationReportController : Controller
    {
        private const string RecommendationReportTemplate = @"~\ExcelTemplates\RecommendationReportTemplate.xls";
        private readonly IReportService _reportService;
        private readonly IExportRecommendationReportService _exportExcelService;

        public ExportRecommendationReportController()
            : this(null, null)
        {
        }

        public ExportRecommendationReportController(
            IReportService reportService,
            IExportRecommendationReportService exportExcelService
            )
        {
            _reportService = reportService ?? new ReportService();
            _exportExcelService = exportExcelService ?? new ExportRecommendationReportService();
        }

        public ActionResult ExportExcel(GetEPARecommendationReportCriteria criteria)
        {
            if (criteria != null)
            {
                EPASummaryRecommendationReportModel data = _reportService.GetRecommendationReport(criteria);

                _exportExcelService.Export(data, Server.MapPath(RecommendationReportTemplate));
            }

            return new EmptyResult();
        }
    }
}