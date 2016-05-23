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
            bool? isLocal = null,
            int? pageIndex = null,
            int? itemsPerPage = null)
        {
            var criteria = new GetEPASummaryCriteria()
            {
                EmployeeId = CurrentUser.EmployeeInfo.EmployeeId,
                PeriodIds = periodIds,
                DepartmentId = departmentId,
                IsLocal = isLocal,
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
            bool? isLocal = null,
            int? pageIndex = null,
            int? itemsPerPage = null)
        {
            var criteria = new GetEPADetailCriteria()
            {
                EmployeeId = CurrentUser.EmployeeInfo.EmployeeId,
                PeriodId = periodId,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                PageIndex = pageIndex,
                ItemsPerPage = itemsPerPage
            };

            return _reportService.GetEPADetailReport(criteria);
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyEPADetailReport, Policy = Policy.Read)]
        public EPADetailModel GetMonthlyEPADetailReport(
            int? periodId = null,
            int? departmentId = null,
            bool? isLocal = null,
            int? pageIndex = null,
            int? itemsPerPage = null)
        {
            var criteria = new GetMonthlyEPADetailCriteria()
            {
                EmployeeId = CurrentUser.EmployeeInfo.EmployeeId,
                PeriodId = periodId,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                PageIndex = pageIndex,
                ItemsPerPage = itemsPerPage
            };

            return _reportService.GetMonthlyEPADetailReport(criteria);
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyEPASummaryReport, Policy = Policy.Read)]
        public EPASummaryModel GetMonthlyEPASummaryReport(
            string periodIds = null,
            int? departmentId = null,
            bool? isLocal = null,
            int? pageIndex = null,
            int? itemsPerPage = null)
        {
            var criteria = new GetMonthlyEPASummaryCriteria()
            {
                EmployeeId = CurrentUser.EmployeeInfo.EmployeeId,
                PeriodIds = periodIds,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                PageIndex = pageIndex,
                ItemsPerPage = itemsPerPage
            };

            return _reportService.GetMonthlyEPASummaryReport(criteria);
        }

        [HttpGet]
        public IList<PeriodModel> GetAllPeriodIncludeImport()
        {
            IList<PeriodModel> periods = _commonController.GetAllPeriodIncludeImport();

            if (periods.Count > 0)
            {
                periods = periods.OrderByDescending(x => x.Year).ToList();
            }

            return periods;
        }
    }
}