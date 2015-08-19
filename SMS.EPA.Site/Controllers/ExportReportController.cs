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
    using System.Web.Mvc;
    using SMS.EPA.Criteria.Period;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class ExportReportController : Controller
    {
        private const string _EmployeeFlowSettingReportTemplatePath = @"~\SourceApp\Components\CheckFlowAssignment\ExcelTemplates\EmployeeFlowSettingReport.xls";

        private readonly IPeriodService _periodService;

        public ExportReportController()
            : this(null)
        {
        }

        public ExportReportController(IPeriodService checkFlowService)
        {
            _periodService = checkFlowService ?? new PeriodService();
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
    }
}