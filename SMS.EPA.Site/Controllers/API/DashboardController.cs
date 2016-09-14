namespace SMS.EPA.Site.Controllers.API
{
    using System.Linq;
    using System.Web.Http;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Criteria.Report;
    using SMS.EPA.Model.Report.Dashboard;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class DashboardController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly IReportService reportService;

        public DashboardController() :
            this(null, null)
        {
        }

        public DashboardController(
            IEmployeeService employeeService,
            IReportService reportService)
        {
            this.employeeService = employeeService ?? new EmployeeService();
            this.reportService = reportService ?? new ReportService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Home, Policy = Policy.Read)]
        public DashboardModel GetDashboard()
        {
            DashboardModel dashboard = new DashboardModel();

            var criteria = new GetDashboardCriteria
            {
                EmployeeId = this.SessionService.EmployeeId
            };

            dashboard = reportService.GetDashboard(criteria);
            dashboard.Employee = employeeService.GetEmployeeById(this.SessionService.EmployeeId);
            dashboard.MonthlyEPAs = dashboard.MyEPAs.Where(x => x.Period.TargetTypeId == (int)TargetTypes.Monthly);
            dashboard.MyEPAs = dashboard.MyEPAs.Where(x => x.Period.TargetTypeId != (int)TargetTypes.Monthly);

            return dashboard;
        }
    }
}