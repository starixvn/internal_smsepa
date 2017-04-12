namespace SMS.EPA.Site.Controllers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Enumerations;
    using Core.Services;
    using Criteria;
    using Criteria.Report;
    using Model;
    using Resources;
    using Services.Implement;
    using Services.Interfaces;

    public class ExportReportController : Controller
    {
        private const string DisciplineSummaryTemplatePath = @"~\ExcelTemplates\DisciplineSummaryReport.xls";
        private const string DisciplineDetailTemplatePath = @"~\ExcelTemplates\DisciplineDetailReport.xls";
        private const string DisciplineSummaryByPeriodTemplatePath = @"~\ExcelTemplates\DisciplineSummaryByPeriodReport.xls";
        private const string UpdateFinalScoreTemplatePath = @"~\ExcelTemplates\UpdateFinalScore.xls";

        private readonly IReportService reportService;
        private readonly IEPADetailReportService epaDetailReportService;
        private readonly IMonthlyEPADetailReportService monthlyEPADetailReportService;
        private readonly IDisciplineService disciplineService;
        private readonly IEmployeeEPAService employeeEPAService;
        private readonly IExportEmployeeFlowSettingService exportEmployeeFlowSettingService;
        private readonly IExportEmployeeInfoService exportEmployeeInfoService;
        private readonly IExportPersonalFlowSettingService exportPersonalFlowSettingService;
        private ISessionService sessionService;

        public ExportReportController()
            : this(null, null, null, null, null, null, null, null, null)
        {
        }

        public ExportReportController(
            IReportService reportService,
            IEPADetailReportService epaDetailReportService,
            IMonthlyEPADetailReportService monthlyEPADetailReportService,
            IDisciplineService disciplineService,
            IEmployeeEPAService employeeEPAService,
            IExportEmployeeFlowSettingService exportEmployeeFlowSettingService,
            IExportEmployeeInfoService exportEmployeeInfoService,
            IExportPersonalFlowSettingService exportPersonalFlowSettingService,
            ISessionService sessionService)
        {
            this.reportService = reportService ?? new ReportService();
            this.epaDetailReportService = epaDetailReportService ?? new EPADetailReportService();
            this.monthlyEPADetailReportService = monthlyEPADetailReportService ?? new MonthlyEPADetailReportService();
            this.disciplineService = disciplineService ?? new DisciplineService();
            this.sessionService = sessionService ?? new SessionService();
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            this.exportEmployeeFlowSettingService = exportEmployeeFlowSettingService ?? new ExportEmployeeFlowSettingService();
            this.exportEmployeeInfoService = exportEmployeeInfoService ?? new ExportEmployeeInfoService();
            this.exportPersonalFlowSettingService = exportPersonalFlowSettingService ?? new ExportPersonalFlowSettingService();
        }

        public ActionResult ExportEmployeeFlowSettingReport(GetPeriodEmployeeSettingCriteria criteria)
        {
            if (criteria != null)
            {
                exportEmployeeFlowSettingService.Export(criteria);
            }

            return new EmptyResult();
        }

        public ActionResult ExportEPASummaryReport(GetEPASummaryCriteria criteria)
        {
            criteria.ActorId = sessionService.CurrentUser.EmployeeId;

            EPASummaryModel epaSummary = reportService.GetEPASummaryReport(criteria);
            epaSummary.Period = reportService.GetEPADetailPeriodForExportedFileName(criteria.PeriodIds);
            
            MemoryStream output = reportService.ExportEPASummaryReport(epaSummary);

            string fileName = GetEPASummaryReportFilename(epaSummary);

            return File(output.ToArray(), "application/vnd.ms-excel", fileName);
        }

        private string GetEPASummaryReportFilename(EPASummaryModel epaSummary)
        {
            string fileName = $"{Lang.EPASummaryReportName}.xls";

            if (!string.IsNullOrEmpty(epaSummary.EpaYear) && !string.IsNullOrEmpty(epaSummary.EpaEvaluationPeriod))
            {
                fileName = $"{epaSummary.EpaYear}_{epaSummary.EpaEvaluationPeriod}_{Lang.EPASummaryReportName}.xls";
            }

            return fileName;
        }

        public ActionResult ExportMonthlyEPASummaryReport(GetMonthlyEPASummaryCriteria criteria)
        {
            string fileName = Lang.MonthlyEPASummaryReportName + ".xls";

            criteria.EmployeeId = sessionService.CurrentUser.EmployeeId;

            EPASummaryModel epaSummary = reportService.GetMonthlyEPASummaryReport(criteria);

            MemoryStream output = reportService.ExportMonthlyEPASummaryReport(epaSummary);

            return File(output.ToArray(), "application/vnd.ms-excel", fileName);
        }

        public ActionResult ExportEPADetailReport(GetEPADetailCriteria criteria)
        {
            criteria.ActorId = sessionService.CurrentUser.EmployeeId;

            EPADetailModel epaDetail = reportService.GetEPADetailReport(criteria);

            epaDetail.Period = reportService.GetEPADetailPeriodForExportedFileName(criteria.PeriodId?.ToString());

            if (string.IsNullOrEmpty(criteria.SortColumnName) && epaDetail.EpaInfos.Any())
            {
                switch (epaDetail.EpaInfos.First().Period.TargetTypeId)
                {
                    case (int)TargetType.Probation:
                        epaDetail.EpaInfos = epaDetail.EpaInfos.OrderByDescending(x => x.Employee.Code);
                        break;

                    default:
                        epaDetail.EpaInfos = epaDetail.EpaInfos.OrderBy(x => x.Employee.GroupCode).ThenBy(x => x.Employee.Code);
                        break;
                }
            }

            epaDetailReportService.Render(epaDetail);

            return new EmptyResult();
        }

        public ActionResult ExportMonthlyEPADetailReport(GetMonthlyEPADetailCriteria criteria)
        {
            criteria.EmployeeId = sessionService.CurrentUser.EmployeeId;

            EPADetailModel epaDetail = reportService.GetMonthlyEPADetailReport(criteria);
            monthlyEPADetailReportService.Render(epaDetail);

            return new EmptyResult();
        }

        public ActionResult ExportDisciplineSummaryReport(GetDisciplineSummaryCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ApproverId = sessionService.CurrentUser.EmployeeId;
                IEnumerable<DisciplineSummaryModel> employeeDisciplines = disciplineService.GetDisciplineSummary(criteria);

                if (employeeDisciplines != null && employeeDisciplines.Any())
                {
                    string templatePath = Server.MapPath(DisciplineSummaryTemplatePath);
                    disciplineService.ExportDisciplineSummaryReport(criteria, employeeDisciplines, templatePath);
                }
            }

            return new EmptyResult();
        }

        public ActionResult ExportDisciplineDetailReport(GetDisciplineDetailCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ApproverId = sessionService.CurrentUser.EmployeeId;
                IEnumerable<DisciplineDetailModel> employeeDisciplines = disciplineService.GetDisciplineDetail(criteria);

                if (employeeDisciplines != null && employeeDisciplines.Any())
                {
                    string templatePath = Server.MapPath(DisciplineDetailTemplatePath);
                    disciplineService.ExportDisciplineDetailReport(criteria, employeeDisciplines, templatePath);
                }
            }

            return new EmptyResult();
        }

        public ActionResult ExportDisciplineSummaryByPeriodReport(GetDisciplineSummaryByPeriodCriteria criteria)
        {
            if (criteria != null)
            {
                IEnumerable<DisciplineSummaryByPeriodModel> employeeDisciplines = disciplineService.GetDisciplineSummaryByPeriod(criteria);

                if (employeeDisciplines != null && employeeDisciplines.Any())
                {
                    string templatePath = Server.MapPath(DisciplineSummaryByPeriodTemplatePath);
                    disciplineService.ExportDisciplineSummaryByPeriodReport(criteria, employeeDisciplines, templatePath);
                }
            }

            return new EmptyResult();
        }

        public ActionResult ExportEPAAfterUpdateFinalScore(GetEPAToUpdateFinalScoreCriteria criteria)
        {
            if (criteria != null)
            {
                IList<EPAModel> epaInfos = employeeEPAService.GetEPAToUpdateFinalScore(criteria);
                string[] employeeIds = criteria.EmployeeIds.Split(',');
                epaInfos = epaInfos.Join(employeeIds, x => x.Employee.EmployeeId.ToString(), y => y, (x, y) => x).ToList();

                if (epaInfos != null && epaInfos.Any())
                {
                    string templatePath = Server.MapPath(UpdateFinalScoreTemplatePath);
                    employeeEPAService.ExportEPAAfterUpdateFinalScore(criteria.PeriodName, epaInfos, templatePath);
                }
            }

            return new EmptyResult();
        }

        public ActionResult ExportEmployeeInfoReport(GetAllEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                exportEmployeeInfoService.Export(criteria);
            }

            return new EmptyResult();
        }

        public ActionResult ExportPersonalFlowSettingReport(int flowTemplateId, string flowTemplateName)
        {
            if (flowTemplateId > 0 && !string.IsNullOrEmpty(flowTemplateName))
            {
                exportPersonalFlowSettingService.Export(flowTemplateId, flowTemplateName);
            }

            return new EmptyResult();
        }
    }
}