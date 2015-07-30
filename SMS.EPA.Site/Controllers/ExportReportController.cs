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
    using SMS.EPA.Criteria.CheckFlowAssignment;
    using SMS.EPA.Model.CheckFlowAssignment;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class ExportReportController : Controller
    {
        private const string _EmployeeFlowSettingReportTemplatePath = @"~\SourceApp\Components\CheckFlowAssignment\ExcelTemplates\EmployeeFlowSettingReport.xls";

        private readonly ICheckFlowAssignmentService _checkFlowService;

        public ExportReportController()
            : this(null)
        {
        }

        public ExportReportController(ICheckFlowAssignmentService checkFlowService)
        {
            _checkFlowService = checkFlowService ?? new CheckFlowAssignmentService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExportEmployeeFlowSettingReport(GetEmployeeFlowSettingCriteria criteria)
        {
            IEnumerable<EmployeeFlowTargetSettingModel> employeeFlowSettings = _checkFlowService.GetEmployeeFlowTargetSetting(criteria);
            string templatePath = Server.MapPath(_EmployeeFlowSettingReportTemplatePath);
            _checkFlowService.ExportEmployeeFlowSettingReport(employeeFlowSettings, templatePath);

            return new EmptyResult();
        }
    }
}