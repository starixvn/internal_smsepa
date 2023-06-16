namespace SMS.EPA.Site.Controllers.Export
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using HRM.UserControl.Client.AspNetMvc.Session;
    using HRM.UserControl.Client.Core.Session;
    using SMS.EPA.Core.Constants;
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
        private readonly IUserControlSession userControlSession;
        private readonly IJobLevelAssessmentService jobLevelAssessmentService;
        private readonly IExportJobLevelAssessmentService exportService;

        public ExportAssessmentController()
            : this(null, null, null, null)
        {
        }

        public ExportAssessmentController(
            IBaseService baseService,
            IUserControlSession userControlSession,
            IJobLevelAssessmentService jobLevelAssessmentService,
            IExportJobLevelAssessmentService exportService)
        {
            this.baseService = baseService ?? new BaseService();
            this.userControlSession = userControlSession ?? new UserControlSession();
            this.jobLevelAssessmentService = jobLevelAssessmentService ?? new JobLevelAssessmentService();
            this.exportService = exportService ?? new ExportJobLevelAssessmentService();
        }

        public ActionResult ExportJoblevelAssessment(GetJobLevelAssessmentReportCriteria criteria)
        {
            criteria.ActorId = userControlSession.Get<int>(Constants.CurrentEmployeeIdKey);
            IList<JobLevelAssessment> assessments = jobLevelAssessmentService.GetJobLevelAssessmentReport(criteria);

            List<JobLevelAssessmentKPI> kpis = assessments
                .SelectMany(ass => ass.KPIs)
                .GroupBy(kpi => new { kpi.KPIName, kpi.KPIId })
                .Select(kpi => kpi.First())
                .OrderBy(kpi => kpi.KPIId)
                .ToList();

            var title = Lang.JoblevelAssessmentReportTitle + " - " + criteria.PeriodName;
            MemoryStream output = exportService.ExportJoblevelAssessmentReport(kpis, assessments, title);

            return File(output.ToArray(), "application/vnd.ms-excel", $"{Lang.JoblevelAssessmentReportName}.xls");
        }
    }
}