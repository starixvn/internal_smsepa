namespace SMS.EPA.Site.Controllers.API
{
    using System.Web.Http;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class DashboardController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly IReportService reportService;
        private readonly IDashboardService dashboardService;

        public DashboardController() :
            this(null, null, null)
        {
        }

        public DashboardController(
            IEmployeeService employeeService,
            IReportService reportService,
            IDashboardService dashboardService)
        {
            this.employeeService = employeeService ?? new EmployeeService();
            this.reportService = reportService ?? new ReportService();
            this.dashboardService = dashboardService ?? new DashboardService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Home, Policy = Policy.Read)]
        public DashboardModel GetDashboard()
        {
            DashboardModel dashboard = reportService.GetDashboard(this.SessionService.EmployeeId);
            dashboard.Employee = employeeService.GetEmployeeById(this.SessionService.EmployeeId);

            return dashboard;
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Home, Policy = Policy.Read)]
        public ToDoListViewModel GetToDoList()
        {
            ToDoListViewModel todoList = dashboardService.GetToDoList(this.SessionService.EmployeeId);

            return todoList;
        }
    }
}