namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Criteria.Report;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Model.Report.EPADetail;
    using SMS.EPA.Model.Report.EPASummary;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class ReportController : BaseController
    {
        private readonly IReportService reportService;
        private readonly CommonController commonController;

        public ReportController()
            : this(null, null)
        {
        }

        public ReportController(
            IReportService reportService,
            CommonController commonController)
        {
            this.reportService = reportService ?? new ReportService();
            this.commonController = commonController ?? new CommonController();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.EPASummaryReport, Policy = Policy.Read)]
        public HttpResponseMessage GetEPASummaryReport(
            string periodIds = null,
            int? departmentId = null,
            bool? isLocal = null,
            string sortColumnName = null,
            string sortDirection = null,
            int? pageIndex = null,
            int? itemsPerPage = null)
        {
            var criteria = new GetEPASummaryCriteria()
            {
                EmployeeId = CurrentUser.EmployeeId,
                PeriodIds = periodIds,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                SortColumnName = sortColumnName,
                SortDirection = sortDirection,
                PageIndex = pageIndex,
                ItemsPerPage = itemsPerPage
            };

            EPASummaryModel epaInfos = reportService.GetEPASummaryReport(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, epaInfos);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.EPADetailReport, Policy = Policy.Read)]
        public HttpResponseMessage GetEPADetailReport(
            int? periodId = null,
            int? departmentId = null,
            bool? isLocal = null,
            string phaseIds = null,
            string sortColumnName = null,
            string sortDirection = null,
            int? pageIndex = null,
            int? itemsPerPage = null)
        {
            var criteria = new GetEPADetailCriteria()
            {
                EmployeeId = CurrentUser.EmployeeId,
                PeriodId = periodId,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                PhaseIds = phaseIds,
                SortColumnName = sortColumnName,
                SortDirection = sortDirection,
                PageIndex = pageIndex,
                ItemsPerPage = itemsPerPage
            };

            EPADetailModel epaInfos = reportService.GetEPADetailReport(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, epaInfos);
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyEPADetailReport, Policy = Policy.Read)]
        public HttpResponseMessage GetMonthlyEPADetailReport(
            int? periodId = null,
            int? departmentId = null,
            bool? isLocal = null,
            string phaseIds = null,
            string sortColumnName = null,
            string sortDirection = null,
            int? pageIndex = null,
            int? itemsPerPage = null)
        {
            var criteria = new GetMonthlyEPADetailCriteria()
            {
                EmployeeId = CurrentUser.EmployeeId,
                PeriodId = periodId,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                PhaseIds = phaseIds,
                SortColumnName = sortColumnName,
                SortDirection = sortDirection,
                PageIndex = pageIndex,
                ItemsPerPage = itemsPerPage
            };

            EPADetailModel epaInfos = reportService.GetMonthlyEPADetailReport(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, epaInfos);
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyEPASummaryReport, Policy = Policy.Read)]
        public HttpResponseMessage GetMonthlyEPASummaryReport(
            string periodIds = null,
            int? departmentId = null,
            bool? isLocal = null,
            string sortColumnName = null,
            string sortDirection = null,
            int? pageIndex = null,
            int? itemsPerPage = null)
        {
            var criteria = new GetMonthlyEPASummaryCriteria()
            {
                EmployeeId = CurrentUser.EmployeeId,
                PeriodIds = periodIds,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                SortColumnName = sortColumnName,
                SortDirection = sortDirection,
                PageIndex = pageIndex,
                ItemsPerPage = itemsPerPage
            };

            EPASummaryModel epaInfos = reportService.GetMonthlyEPASummaryReport(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, epaInfos);
        }

        [HttpGet]
        public IList<PeriodModel> GetAllPeriodIncludeImport()
        {
            IList<PeriodModel> periods = commonController.GetAllPeriodIncludeImport();

            if (periods.Count > 0)
            {
                periods = periods.OrderByDescending(x => x.Year).ToList();
            }

            return periods;
        }
    }
}