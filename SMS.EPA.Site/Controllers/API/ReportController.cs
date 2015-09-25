// -----------------------------------------------------------------------
// <copyright file="ReportController.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.09.25">Created</change>
// </history>

namespace SMS.EPA.Site.Controllers.API
{
    using System.Web.Http;
    using SMS.EPA.Criteria.Report;
    using SMS.EPA.Model.Report.EPASummary;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class ReportController : BaseController
    {
        private readonly IReportService _reportService;

        public ReportController()
            : this(null)
        {
        }

        public ReportController(IReportService reportService)
        {
            _reportService = reportService ?? new ReportService();
        }

        [HttpGet]
        public EPASummaryModel GetEPASummary(
            string periodIds = null,
            int? departmentId = null)
        {
            var criteria = new GetEPASummaryCriteria()
            {
                PeriodIds = periodIds,
                DepartmentId = departmentId
            };

            return _reportService.GetEPASummary(criteria);
        }
    }
}