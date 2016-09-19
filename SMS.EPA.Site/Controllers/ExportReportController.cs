namespace SMS.EPA.Site.Controllers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Criteria.Period;
    using SMS.EPA.Criteria.Report;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Model.Report.EPADetail;
    using SMS.EPA.Model.Report.EPASummary;
    using SMS.EPA.Resources;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class ExportReportController : Controller
    {
        private const string EmployeeFlowSettingReportTemplatePath = @"~\ExcelTemplates\EmployeeFlowSettingReport.xls";
        private const string DisciplineSummaryTemplatePath = @"~\ExcelTemplates\DisciplineSummaryReport.xls";
        private const string DisciplineDetailTemplatePath = @"~\ExcelTemplates\DisciplineDetailReport.xls";
        private const string DisciplineSummaryByPeriodTemplatePath = @"~\ExcelTemplates\DisciplineSummaryByPeriodReport.xls";

        private readonly IPeriodService periodService;
        private readonly IReportService reportService;
        private readonly IAttendanceSummaryService attendanceService;
        private readonly IEPADetailReportService epaDetailReportService;
        private readonly IMonthlyEPADetailReportService monthlyEPADetailReportService;
        private readonly IDisciplineSummaryService disciplineSummaryService;
        private ISessionService sessionService;

        public ExportReportController()
            : this(null, null, null, null, null, null, null)
        {
        }

        public ExportReportController(
            IPeriodService periodService,
            IReportService reportService,
            IAttendanceSummaryService attendanceService,
            IEPADetailReportService epaDetailReportService,
            IMonthlyEPADetailReportService monthlyEPADetailReportService,
            IDisciplineSummaryService disciplineSummaryService,
            ISessionService sessionService)
        {
            this.periodService = periodService ?? new PeriodService();
            this.reportService = reportService ?? new ReportService();
            this.attendanceService = attendanceService ?? new AttendanceSummaryService();
            this.epaDetailReportService = epaDetailReportService ?? new EPADetailReportService();
            this.monthlyEPADetailReportService = monthlyEPADetailReportService ?? new MonthlyEPADetailReportService();
            this.disciplineSummaryService = disciplineSummaryService ?? new DisciplineSummaryService();
            this.sessionService = sessionService ?? new SessionService();
        }

        public ActionResult ExportEmployeeSettingReport(GetPeriodEmployeeSettingCriteria criteria)
        {
            IEnumerable<PeriodEmployeeSettingModel> employeeFlowSettings = null;

            if (criteria != null)
            {
                employeeFlowSettings = periodService.GetPeriodEmployeeSettingById(criteria);

                if (employeeFlowSettings != null)
                {
                    string templatePath = Server.MapPath(EmployeeFlowSettingReportTemplatePath);
                    periodService.ExportEmployeeSettingReport(employeeFlowSettings, templatePath, criteria.PeriodName);
                }
            }

            return new EmptyResult();
        }

        public ActionResult ExportEPASummaryReport(GetEPASummaryCriteria criteria)
        {
            string fileName = Lang.EPASummaryReportName + ".xls";

            criteria.EmployeeId = sessionService.CurrentUser.EmployeeId;

            EPASummaryModel epaSummary = reportService.GetEPASummaryReport(criteria);

            MemoryStream output = reportService.ExportEPASummaryReport(epaSummary);

            return File(output.ToArray(), "application/vnd.ms-excel", fileName);
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
            criteria.EmployeeId = sessionService.CurrentUser.EmployeeId;

            EPADetailModel epaDetail = reportService.GetEPADetailReport(criteria);

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

        public ActionResult ExportAttendanceSummaryReport(GetAttendanceSummaryCriteria criteria)
        {
            IEnumerable<AttendanceSummaryModel> attendanceEmployees = null;

            if (criteria != null)
            {
                criteria.ApproverId = sessionService.CurrentUser.EmployeeId;
                attendanceEmployees = attendanceService.GetAttendanceSummary(criteria);

                if (attendanceEmployees != null && attendanceEmployees.Any())
                {
                    string templatePath = Server.MapPath(DisciplineSummaryTemplatePath);
                    attendanceService.ExportAttendanceSummaryReport(criteria, attendanceEmployees, templatePath);
                }
            }

            return new EmptyResult();
        }

        public ActionResult ExportAttendanceDetailReport(GetDetailAttendanceCriteria criteria)
        {
            IEnumerable<AttendanceDetailModel> attendanceViolations = null;

            if (criteria != null)
            {
                criteria.ApproverId = sessionService.CurrentUser.EmployeeId;
                attendanceViolations = attendanceService.GetAttendanceDetail(criteria);

                if (attendanceViolations != null && attendanceViolations.Any())
                {
                    string templatePath = Server.MapPath(DisciplineDetailTemplatePath);
                    attendanceService.ExportAttendanceDetailReport(criteria, attendanceViolations, templatePath);
                }
            }

            return new EmptyResult();
        }

        public ActionResult ExportDisciplineSummaryByPeriodReport(GetDisciplineSummaryByPeriodCriteria criteria)
        {
            if (criteria != null)
            {
                IEnumerable<DisciplineSummaryModel> employeeDisciplines = disciplineSummaryService.GetDisciplineSummaryByPeriod(criteria);

                if (employeeDisciplines != null && employeeDisciplines.Any())
                {
                    string templatePath = Server.MapPath(DisciplineSummaryByPeriodTemplatePath);
                    disciplineSummaryService.ExportDisciplineSummaryByPeriodReport(criteria, employeeDisciplines, templatePath);
                }
            }

            return new EmptyResult();
        }
    }
}