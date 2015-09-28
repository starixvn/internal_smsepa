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
    using System.Linq;
    using System.Web.Mvc;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Criteria.Period;
    using SMS.EPA.Criteria.Report;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Model.Report.EPASummary;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;
    using SMS.EPA.Web;

    public class ExportReportController : Controller
    {
        private const string _EmployeeFlowSettingReportTemplatePath = @"~\SourceApp\Components\CheckFlowAssignment\ExcelTemplates\EmployeeFlowSettingReport.xls";

        private readonly IPeriodService _periodService;
        private readonly IReportService _reportService;

        public ExportReportController()
            : this(null, null)
        {
        }

        public ExportReportController(
            IPeriodService checkFlowService,
            IReportService reportService)
        {
            _periodService = checkFlowService ?? new PeriodService();
            _reportService = reportService ?? new ReportService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExportEmployeeSettingReport(GetPeriodEmployeeSettingCriteria criteria)
        {
            IEnumerable<PeriodEmployeeSettingModel> employeeFlowSettings = _periodService.GetPeriodEmployeeSettingById(criteria);
            string templatePath = Server.MapPath(_EmployeeFlowSettingReportTemplatePath);
            _periodService.ExportEmployeeSettingReport(employeeFlowSettings, templatePath);

            return new EmptyResult();
        }

        public ActionResult ExportEPASummaryReport(GetEPASummaryCriteria criteria)
        {
            string fileName = "EPA_Summary_Report.xls";
            List<int> roleIds = SessionManager.CurrentUser.Roles.Select(x => x.RoleId).ToList();
            var isViewAll = false;

            if (roleIds.Contains((int)SystemRole.SystemAdmin)
                || roleIds.Contains((int)SystemRole.HumanResource)
                || roleIds.Contains((int)SystemRole.Management))
            {
                isViewAll = true;
            }

            criteria.EmployeeId = SessionManager.CurrentUser.EmployeeInfo.EmployeeId;
            criteria.IsViewAll = isViewAll;

            EPASummaryModel epaSummary = _reportService.GetEPASummary(criteria);
            MemoryStream output = _reportService.ExportEPASummaryReport(epaSummary);

            return File(output.ToArray(), "application/vnd.ms-excel", fileName);
        }
    }
}