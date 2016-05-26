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
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Criteria.AttendanceSummary;
    using SMS.EPA.Criteria.Period;
    using SMS.EPA.Criteria.Report;
    using SMS.EPA.Model.AttendanceSummary;
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
        private const string _MonthlyEPADetailReportTemplatePath = @"~\SourceApp\Components\Report\ExcelTemplates\MonthlyEPADetailReport.xls";
        private const string _DisciplineSummaryTemplatePath = @"~\SourceApp\Components\AttendanceSummary\ExcelTemplates\DisciplineSummaryReport.xls";
        private const string _DisciplineDetailTemplatePath = @"~\SourceApp\Components\AttendanceSummary\ExcelTemplates\DisciplineDetailReport.xls";

        private readonly IPeriodService _periodService;
        private readonly IReportService _reportService;
        private readonly IAttendanceSummaryService _attendanceService;
        private readonly IEPADetailReportService _epaDetailReportService;

        public ExportReportController()
            : this(null, null, null, null)
        {
        }

        public ExportReportController(
            IPeriodService checkFlowService,
            IReportService reportService,
            IAttendanceSummaryService attendanceService,
            IEPADetailReportService epaDetailReportService)
        {
            _periodService = checkFlowService ?? new PeriodService();
            _reportService = reportService ?? new ReportService();
            _attendanceService = attendanceService ?? new AttendanceSummaryService();
            _epaDetailReportService = epaDetailReportService ?? new EPADetailReportService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExportEmployeeSettingReport(GetPeriodEmployeeSettingCriteria criteria)
        {
            IEnumerable<PeriodEmployeeSettingModel> employeeFlowSettings = null;

            employeeFlowSettings = SessionManager.Get<IEnumerable<PeriodEmployeeSettingModel>>(
                Constants.PeriodEmployeeSettingReportSessionKey + SessionManager.CurrentUser.EmployeeInfo.EmployeeId);

            if (criteria != null)
            {
                if (employeeFlowSettings == null)
                {
                    employeeFlowSettings = _periodService.GetPeriodEmployeeSettingById(criteria);
                }

                if (employeeFlowSettings != null)
                {
                    string templatePath = Server.MapPath(_EmployeeFlowSettingReportTemplatePath);
                    _periodService.ExportEmployeeSettingReport(employeeFlowSettings, templatePath, criteria.PeriodName);
                }
            }

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

        public ActionResult ExportMonthlyEPASummaryReport(GetMonthlyEPASummaryCriteria criteria)
        {
            string fileName = Lang.MonthlyEPASummaryReportName + ".xls";

            criteria.EmployeeId = SessionManager.CurrentUser.EmployeeInfo.EmployeeId;

            EPASummaryModel epaSummary = _reportService.GetMonthlyEPASummaryReport(criteria);

            MemoryStream output = _reportService.ExportMonthlyEPASummaryReport(epaSummary);

            return File(output.ToArray(), "application/vnd.ms-excel", fileName);
        }

        public ActionResult ExportEPADetailReport(GetEPADetailCriteria criteria)
        {
            criteria.EmployeeId = SessionManager.CurrentUser.EmployeeInfo.EmployeeId;

            EPADetailModel epaDetail = _reportService.GetEPADetailReport(criteria);

            _epaDetailReportService.Render(epaDetail);

            return new EmptyResult();
        }

        public ActionResult ExportMonthlyEPADetailReport(GetMonthlyEPADetailCriteria criteria)
        {
            criteria.EmployeeId = SessionManager.CurrentUser.EmployeeInfo.EmployeeId;

            EPADetailModel epaDetail = _reportService.GetMonthlyEPADetailReport(criteria);
            string templatePath = Server.MapPath(_MonthlyEPADetailReportTemplatePath);
            _reportService.ExportMonthlyEPADetailReport(epaDetail, templatePath);

            return new EmptyResult();
        }

        public ActionResult ExportAttendanceSummaryReport(GetAttendanceSummaryCriteria criteria)
        {
            IEnumerable<AttendanceSummaryModel> attendanceEmployees = null;
            attendanceEmployees = SessionManager.Get<IEnumerable<AttendanceSummaryModel>>(
                Constants.DisciplineSummaryReportSessionKey + SessionManager.CurrentUser.EmployeeInfo.EmployeeId);

            if (attendanceEmployees == null && criteria != null)
            {
                criteria.ApproverId = SessionManager.CurrentUser.EmployeeInfo.EmployeeId;
                attendanceEmployees = _attendanceService.GetAttendanceSummary(criteria);
            }

            if (attendanceEmployees != null)
            {
                string templatePath = Server.MapPath(_DisciplineSummaryTemplatePath);
                _attendanceService.ExportAttendanceSummaryReport(criteria, attendanceEmployees, templatePath);
            }

            return new EmptyResult();
        }

        public ActionResult ExportAttendanceDetailReport(GetDetailAttendanceCriteria criteria)
        {
            IEnumerable<AttendanceDetailModel> attendanceViolations = null;
            attendanceViolations = SessionManager.Get<IEnumerable<AttendanceDetailModel>>(
                Constants.DisciplineDetailReportSessionKey + SessionManager.CurrentUser.EmployeeInfo.EmployeeId);

            if (criteria != null)
            {
                if (criteria.HasChanged || attendanceViolations == null)
                {
                    criteria.ApproverId = SessionManager.CurrentUser.EmployeeInfo.EmployeeId;
                    attendanceViolations = _attendanceService.GetAttendanceDetail(criteria);
                }

                if (attendanceViolations != null)
                {
                    string templatePath = Server.MapPath(_DisciplineDetailTemplatePath);
                    _attendanceService.ExportAttendanceDetailReport(criteria, attendanceViolations, templatePath);
                }
            }

            return new EmptyResult();
        }
    }
}