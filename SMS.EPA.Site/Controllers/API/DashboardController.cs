namespace SMS.EPA.Site.Controllers.API
{
    using Core.Attribute;
    using Core.Constants;
    using Core.Enumerations;
    using Model;
    using Services.Implement;
    using Services.Interfaces;
    using System.Web.Http;

    public class DashboardController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly IReportService reportService;
        private readonly IDashboardService dashboardService;

        public DashboardController() : this(null, null, null)
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
            DashboardModel dashboard = reportService.GetDashboard(SessionService.EmployeeId);
            dashboard.Employee = employeeService.GetEmployeeById(SessionService.EmployeeId);

            return dashboard;
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Home, Policy = Policy.Read)]
        public ToDoListViewModel GetToDoList()
        {
            ToDoListViewModel todoList = dashboardService.GetToDoList(SessionService.EmployeeId);

            return todoList;
        }
    }
}