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
    using System.Web.Mvc;
    using SMS.EPA.Criteria.CheckFlowAssignment;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class ExportReportController : Controller
    {
        private const string _CheckAssignmentReportTemplatePath = @"~\SourceApp\Components\CheckFlowAssignment\ExcelTemplates\CheckAssignmentReport.xls";

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

        public ActionResult ExportCheckAssignmentReport(GetEmployeeFlowSettingCriteria criteria)
        {
            return new EmptyResult();
        }
    }
}
