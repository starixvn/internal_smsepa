namespace SMS.EPA.Site.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Criteria.Report;
    using SMS.EPA.Model.Report.Feedback;
    using SMS.EPA.Services;

    public class ExportFeedbackReportController : Controller
    {
        private readonly IFeedbackReportService feedbackReportService;

        public ExportFeedbackReportController() : this(null)
        {
        }

        public ExportFeedbackReportController(IFeedbackReportService feedbackReportService)
        {
            this.feedbackReportService = feedbackReportService ?? new FeedbackReportService();
        }

        [HttpPost]
        public ActionResult ExportExcel(GetFeedbackReportCriteria criteria)
        {
            IEnumerable<FeedbackReport> employees = feedbackReportService.GetData(criteria);
            byte[] bytes = feedbackReportService.Export(employees, criteria.Period);
            const string fileName = "360 Degree EPA Report.xlsx";
            return File(bytes, "application/xlsx", fileName);
        }
    }
}