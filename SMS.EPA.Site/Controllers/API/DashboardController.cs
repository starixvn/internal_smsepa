namespace SMS.EPA.Site.Controllers.API
{
    using System.Web.Http;
    using Core.Attribute;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Services;
    using Criteria;
    using Model;
    using Services.Implement;
    using Services.Interfaces;

    public class DashboardController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly IReportService reportService;
        private readonly IBaseService baseService;

        public DashboardController() : this(null, null, null)
        {
        }

        public DashboardController(
            IEmployeeService employeeService,
            IReportService reportService,
            IBaseService baseService)
        {
            this.employeeService = employeeService ?? new EmployeeService();
            this.reportService = reportService ?? new ReportService();
            this.baseService = baseService ?? new BaseService();
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
            var criteria = new GetToDoListCriteria
            {
                EmployeeId = SessionService.EmployeeId
            };
            var result = new ToDoListViewModel { ToDoList = baseService.GetData<ToDoListModel>(criteria) };

            return result;
        }
    }
}