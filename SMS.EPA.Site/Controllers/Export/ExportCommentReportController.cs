namespace SMS.EPA.Site.Controllers
{
    using System.Web.Mvc;
    using Model;
    using Services.Implement;
    using Services.Interfaces;

    public class ExportCommentReportController : Controller
    {
        private const string CommentReportTemplate = @"~\ExcelTemplates\CommentReportTemplate.xls";
        private readonly ICommentReportService _commentReportService;
        private readonly IExportCommentReportService _exportExcelService;

        public ExportCommentReportController()
            : this(null, null)
        {
        }

        public ExportCommentReportController(
            ICommentReportService commentReportService,
            IExportCommentReportService exportExcelService
            )
        {
            _commentReportService = commentReportService ?? new CommentReportService();
            _exportExcelService = exportExcelService ?? new ExportCommentReportService();
        }

        public ActionResult ExportExcel(int periodId, string periodName)
        {
            var data = new EPASummaryCommentReportModel
            {
                CommentSummary = _commentReportService.GetSummaryComment(periodId),
                PeriodId = periodId,
                PeriodName = periodName
            };

            _exportExcelService.Export(data, Server.MapPath(CommentReportTemplate));

            return new EmptyResult();
        }
    }
}