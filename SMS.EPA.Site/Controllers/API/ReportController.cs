namespace SMS.EPA.Site.Controllers.API
{
    using Core.Attribute;
    using Core.Constants;
    using Core.Enumerations;
    using Criteria.Report;
    using Model;
    using Services.Implement;
    using Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

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
                ActorId = SessionService.EmployeeId,
                EmployeeId = employeeId,
                PeriodIds = periodIds,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                SortColumnName = sortColumnName,
                SortDirection = sortDirection
            };

            EPASummaryModel epaInfos = _reportService.GetEPASummaryReport(criteria);

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
                ActorId = SessionService.EmployeeId,
                EmployeeId = employeeId,
                PeriodId = periodId,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                PhaseIds = phaseIds,
                SortColumnName = sortColumnName,
                SortDirection = sortDirection
            };

            EPADetailModel epaInfos = _reportService.GetEPADetailReport(criteria);

            if (string.IsNullOrEmpty(sortColumnName) && epaInfos.EpaInfos.Any())
            {
                switch (epaInfos.EpaInfos.First().Period.TargetTypeId)
                {
                    case (int)TargetType.Probation:
                        epaInfos.EpaInfos = epaInfos.EpaInfos.OrderByDescending(x => x.Employee.Code);
                        break;

                    default:
                        epaInfos.EpaInfos = epaInfos.EpaInfos.OrderBy(x => x.Employee.GroupCode)
                            .ThenBy(x => x.Employee.Code);
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
                EmployeeId = SessionService.EmployeeId,
                PeriodId = periodId,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                PhaseIds = phaseIds,
                SortColumnName = sortColumnName,
                SortDirection = sortDirection
            };

            EPADetailModel epaInfos = _reportService.GetMonthlyEPADetailReport(criteria);

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
                EmployeeId = SessionService.EmployeeId,
                PeriodIds = periodIds,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                SortColumnName = sortColumnName,
                SortDirection = sortDirection
            };

            EPASummaryModel epaInfos = _reportService.GetMonthlyEPASummaryReport(criteria);

            return Request.CreateResponse(HttpStatusCode.OK, epaInfos);
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
                ActorId = SessionService.EmployeeId
            };

            IEnumerable<EmployeeViewModel> employees = _reportService.GetEmployeeByActorId(criteria);

            return employees;
        }
    }
}