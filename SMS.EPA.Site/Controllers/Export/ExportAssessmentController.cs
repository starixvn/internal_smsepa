namespace SMS.EPA.Site.Controllers.Export
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using iTextSharp.tool.xml.html.head;
    using NPOI.SS.Formula.Functions;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Criteria.JobLevelStandard;
    using SMS.EPA.Criteria.JobLevelStandard.Report;
    using SMS.EPA.Model;
    using SMS.EPA.Model.JobLevelStandard;
    using SMS.EPA.Resources;
    using SMS.EPA.Services.Implement.ExportFile;
    using SMS.EPA.Services.Interfaces.ExportFile;
    using SMS.EPA.Site.Controllers.API;

    public class ExportAssessmentController : Controller
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;
        private readonly IExportJobLevelAssessmentService exportJobLevelAssessmentService;

        public ExportAssessmentController()
            : this(null, null, null)
        {
        }

        public ExportAssessmentController(
            IBaseService baseService,
            ISessionService sessionService,
            IExportJobLevelAssessmentService exportJobLevelAssessmentService)
        {
            this.baseService = baseService ?? new BaseService();
            this.sessionService = sessionService ?? new SessionService();
            this.exportJobLevelAssessmentService = exportJobLevelAssessmentService ?? new ExportJobLevelAssessmentService();
        }
        public ActionResult ExportJoblevelAssessment(GetJobLevelAssessmentReportCriteria criteria)
        {
            var kpicriteria = new GetKPIByPeriodIdCriteria { PeriodId = criteria.PeriodId };
            var kpis = baseService.GetData<JobLevelStandardKPI>(kpicriteria).ToList();

            var settingCriteria = new GetPeriodJobLevelSettingByPeriodIdCriteria { PeriodId = criteria.PeriodId };
            var jobLevelSettings = baseService.GetData<JobLevelSetting>(settingCriteria).ToList();

            criteria.ActorId = sessionService.CurrentUser.EmployeeId;
            var tuple = baseService.FetchMultiple<JobLevelAssessment, JobLevelAssessmentKPI>(criteria);
            IList<JobLevelAssessment> assessments = tuple.Item1.ToList();
            foreach (var assessment in assessments)
            {
                assessment.RequirementMinScore = jobLevelSettings
                    .FirstOrDefault(x => x.SectionId == assessment.SectionId && x.JobLevelId == assessment.CurrentJobLevelId)?.MinWeight;
                assessment.KPIs = tuple.Item2.Where(x => x.EmployeeId == assessment.EmployeeId).ToList();

                if (assessment.TotalWeight.HasValue)
                {
                    assessment.SystemPropose = assessment.TotalWeight.Value - (assessment.RequirementMinScore ?? 0);
                }
            }
            var title = Lang.JoblevelAssessmentReportTitle + " - " + criteria.PeriodName;
            MemoryStream output = exportJobLevelAssessmentService.ExportJoblevelAssessmentReport(kpis, assessments, title);


            return File(output.ToArray(), "application/vnd.ms-excel", $"{Lang.JoblevelAssessmentReportName}.xls");
        }
    }
}