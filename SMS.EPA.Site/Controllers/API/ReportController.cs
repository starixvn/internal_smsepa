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
    using SMS.EPA.Model;
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
            int? employeeId = null,
            string sortColumnName = null,
            string sortDirection = null)
        {
            var criteria = new GetEPASummaryCriteria()
            {
                ActorId = this.SessionService.EmployeeId,
                EmployeeId = employeeId,
                PeriodIds = periodIds,
                DepartmentId = departmentId,
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
            int? departmentId = null,
            bool? isLocal = null,
            string phaseIds = null,
            int? employeeId = null,
            string sortColumnName = null,
            string sortDirection = null)
        {
            var criteria = new GetEPADetailCriteria()
            {
                ActorId = this.SessionService.EmployeeId,
                EmployeeId = employeeId,
                PeriodId = periodId,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                PhaseIds = phaseIds,
                SortColumnName = sortColumnName,
                SortDirection = sortDirection
            };

            EPADetailModel epaInfos = reportService.GetEPADetailReport(criteria);

            if (string.IsNullOrEmpty(sortColumnName) && epaInfos.EpaInfos.Any())
            {
                switch (epaInfos.EpaInfos.First().Period.TargetTypeId)
                {
                    case (int)TargetType.Probation:
                        epaInfos.EpaInfos = epaInfos.EpaInfos.OrderByDescending(x => x.Employee.Code);
                        break;

                    default:
                        epaInfos.EpaInfos = epaInfos.EpaInfos.OrderBy(x => x.Employee.GroupCode).ThenBy(x => x.Employee.Code);
                        break;
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, epaInfos);
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.MonthlyEPADetailReport, Policy = Policy.Read)]
        public HttpResponseMessage GetMonthlyEPADetailReport(
            int? periodId = null,
            int? departmentId = null,
            bool? isLocal = null,
            string phaseIds = null,
            string sortColumnName = null,
            string sortDirection = null)
        {
            var criteria = new GetMonthlyEPADetailCriteria()
            {
                EmployeeId = this.SessionService.EmployeeId,
                PeriodId = periodId,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                PhaseIds = phaseIds,
                SortColumnName = sortColumnName,
                SortDirection = sortDirection
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
            string sortDirection = null)
        {
            var criteria = new GetMonthlyEPASummaryCriteria()
            {
                EmployeeId = this.SessionService.EmployeeId,
                PeriodIds = periodIds,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                SortColumnName = sortColumnName,
                SortDirection = sortDirection
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

        public IEnumerable<EmployeeViewModel> GetEmployeeByActorId(
            string query = "",
            string periodIds = null,
            int? departmentId = null,
            bool? isLocal = null)
        {
            var criteria = new GetEmployeeByActorIdCriteria
            {
                Query = query,
                PeriodIds = periodIds,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                ActorId = this.SessionService.EmployeeId
            };

            IEnumerable<EmployeeViewModel> employees = reportService.GetEmployeeByActorId(criteria);

            return employees;
        }
    }
}