// ----------------------------------------------------------------------
// <copyright file="ExportReportController.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/28/2015">Created.</change>
// </history>

namespace SMS.EPA.Site.Controllers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Mvc;
    using SMS.EPA.Criteria.AttendanceSummary;
    using SMS.EPA.Criteria.Period;
    using SMS.EPA.Criteria.Report;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Model.Report.EPADetail;
    using SMS.EPA.Model.Report.EPASummary;
    using SMS.EPA.Resources;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;
    using SMS.EPA.Web;

    public class ExportReportController : Controller
    {
        private const string _EmployeeFlowSettingReportTemplatePath = @"~\SourceApp\Components\Period\ExcelTemplates\EmployeeFlowSettingReport.xls";
        private const string _EPADetailReportTemplatePath = @"~\SourceApp\Components\Report\ExcelTemplates\EPADetailReport.xls";
        private const string _DisciplineSummaryTemplatePath = @"~\SourceApp\Components\AttendanceSummary\ExcelTemplates\DisciplineSummaryReport.xls";
        private const string _DisciplineDetailTemplatePath = @"~\SourceApp\Components\AttendanceSummary\ExcelTemplates\DisciplineDetailReport.xls";

        private readonly IPeriodService _periodService;
        private readonly IReportService _reportService;
        private readonly IAttendanceSummaryService _attendanceService;

        public ExportReportController()
            : this(null, null, null)
        {
        }

        public ExportReportController(
            IPeriodService checkFlowService,
            IReportService reportService,
            IAttendanceSummaryService attendanceService)
        {
            _periodService = checkFlowService ?? new PeriodService();
            _reportService = reportService ?? new ReportService();
            _attendanceService = attendanceService ?? new AttendanceSummaryService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExportEmployeeSettingReport(GetPeriodEmployeeSettingCriteria criteria)
        {
            IEnumerable<PeriodEmployeeSettingModel> employeeFlowSettings = _periodService.GetPeriodEmployeeSettingById(criteria);
            string templatePath = Server.MapPath(_EmployeeFlowSettingReportTemplatePath);
            _periodService.ExportEmployeeSettingReport(employeeFlowSettings, templatePath, criteria.PeriodName);

            return new EmptyResult();
        }

        public ActionResult ExportEPASummaryReport(GetEPASummaryCriteria criteria)
        {
            string fileName = Lang.EPASummaryReportName + ".xls";

            criteria.EmployeeId = SessionManager.CurrentUser.EmployeeInfo.EmployeeId;

            EPASummaryModel epaSummary = _reportService.GetEPASummaryReport(criteria);
            MemoryStream output = _reportService.ExportEPASummaryReport(epaSummary);

            return File(output.ToArray(), "application/vnd.ms-excel", fileName);
        }

        public ActionResult ExportEPADetailReport(GetEPADetailCriteria criteria)
        {
            criteria.EmployeeId = SessionManager.CurrentUser.EmployeeInfo.EmployeeId;

            EPADetailModel epaDetail = _reportService.GetEPADetailReport(criteria);
            string templatePath = Server.MapPath(_EPADetailReportTemplatePath);
            _reportService.ExportEPADetailReport(epaDetail, templatePath);

            return new EmptyResult();
        }

        public ActionResult ExportAttendanceSummaryReport(GetAttendanceSummaryCriteria criteria)
        {
            string templatePath = Server.MapPath(_DisciplineSummaryTemplatePath);

            if (criteria != null)
            {
                criteria.ApproverId = SessionManager.CurrentUser.EmployeeInfo.EmployeeId;
                _attendanceService.ExportAttendanceSummaryReport(criteria, templatePath);
            }

            return new EmptyResult();
        }

        public ActionResult ExportAttendanceDetailReport(GetDetailAttendanceCriteria criteria)
        {
            string templatePath = Server.MapPath(_DisciplineDetailTemplatePath);

            if (criteria != null)
            {
                criteria.ApproverId = SessionManager.CurrentUser.EmployeeInfo.EmployeeId;
                _attendanceService.ExportAttendanceDetailReport(criteria, templatePath);
            }

            return new EmptyResult();
        }
    }
}