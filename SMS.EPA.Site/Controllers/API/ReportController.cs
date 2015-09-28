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
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using SMS.EPA.Core.Enumerations;
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
            int? departmentId = null,
            int? pageIndex = null,
            int? itemsPerPage = null)
        {
            List<int> roleIds = CurrentUser.Roles.Select(x => x.RoleId).ToList();
            var isViewAll = false;
            if (roleIds.Contains((int)SystemRole.SystemAdmin)
                || roleIds.Contains((int)SystemRole.HumanResource)
                || roleIds.Contains((int)SystemRole.Management))
            {
                isViewAll = true;
            }

            var criteria = new GetEPASummaryCriteria()
            {
                EmployeeId = CurrentUser.EmployeeInfo.EmployeeId,
                IsViewAll = isViewAll,
                PeriodIds = periodIds,
                DepartmentId = departmentId,
                PageIndex = pageIndex,
                ItemsPerPage = itemsPerPage
            };

            return _reportService.GetEPASummary(criteria);
        }
    }
}