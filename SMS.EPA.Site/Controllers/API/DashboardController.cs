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
        private readonly IEmployeeService _employeeService;
        private readonly IReportService _reportService;
        private readonly IDashboardService _dashboardService;

        public DashboardController() :
            this(null, null, null)
        {
        }

        public DashboardController(
            IEmployeeService employeeService,
            IReportService reportService,
            IDashboardService dashboardService)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _reportService = reportService ?? new ReportService();
            _dashboardService = dashboardService ?? new DashboardService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Home, Policy = Policy.Read)]
        public DashboardModel GetDashboard()
        {
            DashboardModel dashboard = _reportService.GetDashboard(SessionService.EmployeeId);
            dashboard.Employee = _employeeService.GetEmployeeById(SessionService.EmployeeId);

            return dashboard;
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Home, Policy = Policy.Read)]
        public ToDoListViewModel GetToDoList()
        {
            ToDoListViewModel todoList = _dashboardService.GetToDoList(SessionService.EmployeeId);

            return todoList;
        }
    }
}