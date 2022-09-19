namespace SMS.EPA.Site.Controllers.Export
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.JobLevelStandard;
    using SMS.EPA.Criteria.JobLevelStandard.Report;
    using SMS.EPA.Model.JobLevelStandard;
    using SMS.EPA.Resources;
    using SMS.EPA.Services;
    using SMS.EPA.Services.Implement.ExportFile;
    using SMS.EPA.Services.Interfaces.ExportFile;

    public class ExportAssessmentController : Controller
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;
        private readonly IJoblevelAssessmentReportService reportService;
        private readonly IExportJobLevelAssessmentService exportService;

        public ExportAssessmentController()
            : this(null, null, null, null)
        {
        }

        public ExportAssessmentController(
            IBaseService baseService,
            ISessionService sessionService,
            IJoblevelAssessmentReportService reportService,
            IExportJobLevelAssessmentService exportService)
        {
            this.baseService = baseService ?? new BaseService();
            this.sessionService = sessionService ?? new SessionService();
            this.reportService = reportService ?? new JoblevelAssessmentReportService();
            this.exportService = exportService ?? new ExportJobLevelAssessmentService();
        }

        public ActionResult ExportJoblevelAssessment(GetJobLevelAssessmentReportCriteria criteria)
        {
            var kpicriteria = new GetKPIByPeriodIdCriteria { PeriodId = criteria.PeriodId };
            var kpis = baseService.GetData<JobLevelStandardKPI>(kpicriteria).ToList();

            criteria.ActorId = sessionService.CurrentUser.EmployeeId;
            IList<JobLevelAssessment> assessments = reportService.GetJoblevelAssessment(criteria);
            var title = Lang.JoblevelAssessmentReportTitle + " - " + criteria.PeriodName;
            MemoryStream output = exportService.ExportJoblevelAssessmentReport(kpis, assessments, title);

            return File(output.ToArray(), "application/vnd.ms-excel", $"{Lang.JoblevelAssessmentReportName}.xls");
        }
    }
}