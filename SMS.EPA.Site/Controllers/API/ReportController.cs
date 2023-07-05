namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models.Role;
    using HRM.UserControl.Client.Core.Services;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Criteria.EPAHistory;
    using SMS.EPA.Criteria.Report;
    using SMS.EPA.Model;
    using SMS.EPA.Services;

    public class ReportController : BaseController
    {
        private readonly IRoleService roleService;
        private readonly IReportService reportService;
        private readonly ICommonService commonService;

        public ReportController() : this(null, null, null)
        {
        }

        public ReportController(
            IRoleService roleService,
            IReportService reportService,
            ICommonService commonService)
        {
            this.roleService = roleService ?? new RoleService();
            this.reportService = reportService ?? new ReportService();
            this.commonService = commonService ?? new CommonService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.EPASummaryReport, Policy = Policy.Read)]
        public HttpResponseMessage GetEPASummaryReport(
            string periodIds = null,
            string teamIds = null,
            bool? isLocal = null,
            int? employeeId = null,
            string sortColumnName = null,
            string sortDirection = null)
        {
            var criteria = new GetEPASummaryCriteria
            {
                ActorId = CurrentEmployeeId,
                EmployeeId = employeeId,
                PeriodIds = periodIds,
                TeamIds = teamIds,
                IsLocal = isLocal,
                SortColumnName = sortColumnName,
                SortDirection = sortDirection
            };

            EPASummaryModel epaInfos = reportService.GetEPASummaryReport(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, epaInfos);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.EPADetailReport, Policy = Policy.Read)]
        public HttpResponseMessage GetEPADetailReport(
            int? periodId = null,
            string teamIds = null,
            bool? isLocal = null,
            string phaseIds = null,
            int? employeeId = null,
            string sortColumnName = null,
            string sortDirection = null)
        {
            var criteria = new GetEPADetailCriteria
            {
                ActorId = CurrentEmployeeId,
                EmployeeId = employeeId,
                PeriodId = periodId,
                TeamIds = teamIds,
                IsLocal = isLocal,
                PhaseIds = phaseIds,
                SortColumnName = sortColumnName,
                SortDirection = sortDirection
            };

            IList<Role> roles = roleService.GetBySiteId(UserControlSession.SiteId);
            EPADetailModel epaInfos = reportService.GetEPADetailReport(criteria, CurrentEmployeeId, roles);

            if (string.IsNullOrEmpty(sortColumnName) && epaInfos.EpaInfos.Any())
            {
                epaInfos.EpaInfos = epaInfos.EpaInfos.First().Period.TargetTypeId == (byte)TargetType.Probation
                    ? epaInfos.EpaInfos.OrderByDescending(x => x.Employee.Code)
                    : epaInfos.EpaInfos.OrderBy(x => x.Employee.GroupCode).ThenBy(x => x.Employee.Code);
            }

            return Request.CreateResponse(HttpStatusCode.OK, epaInfos);
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyEPADetailReport, Policy = Policy.Read)]
        public HttpResponseMessage GetMonthlyEPADetailReport(
            int? periodId = null,
            string teamIds = null,
            bool? isLocal = null,
            string phaseIds = null,
            string sortColumnName = null,
            string sortDirection = null)
        {
            var criteria = new GetMonthlyEPADetailCriteria
            {
                EmployeeId = CurrentEmployeeId,
                PeriodId = periodId,
                TeamIds = teamIds,
                IsLocal = isLocal,
                PhaseIds = phaseIds,
                SortColumnName = sortColumnName,
                SortDirection = sortDirection
            };

            IList<Role> roles = roleService.GetBySiteId(UserControlSession.SiteId);
            EPADetailModel epaInfos = reportService.GetMonthlyEPADetailReport(criteria, CurrentEmployeeId, roles);

            return Request.CreateResponse(HttpStatusCode.OK, epaInfos);
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyEPASummaryReport, Policy = Policy.Read)]
        public HttpResponseMessage GetMonthlyEPASummaryReport(
            string periodIds = null,
            string teamIds = null,
            bool? isLocal = null,
            string sortColumnName = null,
            string sortDirection = null)
        {
            var criteria = new GetMonthlyEPASummaryCriteria
            {
                EmployeeId = CurrentEmployeeId,
                PeriodIds = periodIds,
                TeamIds = teamIds,
                IsLocal = isLocal,
                SortColumnName = sortColumnName,
                SortDirection = sortDirection
            };

            EPASummaryModel epaInfos = reportService.GetMonthlyEPASummaryReport(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, epaInfos);
        }

        [HttpGet]
        public IEnumerable<PeriodModel> GetAllPeriodIncludeImport()
        {
            var criteria = new GetAllPeriodIncludeImportCriteria();
            IEnumerable<PeriodModel> periods = commonService.GetAllPeriodIncludeImport(criteria);

            if (periods.Any())
            {
                periods = periods.OrderByDescending(x => x.Year).ToList();
            }

            return periods;
        }

        public IEnumerable<EmployeeViewModel> GetEmployeeByActorId(
            string query = "",
            string periodIds = null,
            string teamIds = null,
            bool? isLocal = null)
        {
            var criteria = new GetEmployeeByActorIdCriteria
            {
                Query = query,
                PeriodIds = periodIds,
                TeamIds = teamIds,
                IsLocal = isLocal,
                ActorId = CurrentEmployeeId
            };
            return reportService.GetEmployeeByActorId(criteria);
        }
    }
}