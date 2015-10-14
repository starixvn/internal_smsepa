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
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Criteria.Report;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Model.Report.EPADetail;
    using SMS.EPA.Model.Report.EPASummary;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;
    using SMS.EPA.Web.ApiAuthorizeAttribute;

    public class ReportController : BaseController
    {
        private readonly IReportService _reportService;
        private readonly CommonController _commonController;

        public ReportController()
            : this(null, null)
        {
        }

        public ReportController(
            IReportService reportService,
            CommonController commonController)
        {
            _reportService = reportService ?? new ReportService();
            _commonController = commonController ?? new CommonController();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.EPASummaryReport, Policy = Policy.Read)]
        public EPASummaryModel GetEPASummaryReport(
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

            return _reportService.GetEPASummaryReport(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.EPADetailReport, Policy = Policy.Read)]
        public EPADetailModel GetEPADetailReport(
            int? periodId = null,
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

            var criteria = new GetEPADetailCriteria()
            {
                EmployeeId = CurrentUser.EmployeeInfo.EmployeeId,
                IsViewAll = isViewAll,
                PeriodId = periodId,
                DepartmentId = departmentId,
                PageIndex = pageIndex,
                ItemsPerPage = itemsPerPage
            };

            return _reportService.GetEPADetailReport(criteria);
        }

        [HttpGet]
        public IList<PeriodModel> GetPeriod(
            int? targetTypeId = null,
            bool? isActive = true)
        {
            IList<PeriodModel> periods = _commonController.GetPeriod(targetTypeId, isActive)
                .OrderBy(x => x.StartDate).ToList();

            return periods;
        }
    }
}