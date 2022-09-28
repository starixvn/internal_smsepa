namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using Core.Attribute;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Services;
    using Criteria;
    using Model;
    using Services.Implement;
    using Services.Interfaces;
    using SMS.EPA.Criteria.Dashboard;
    using SMS.EPA.Model.Dashboard;

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

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Home, Policy = Policy.Read)]
        public IList<string> GetAnnouncement()
        {
            var result = new List<string>();
            var criteria = new GetAnnouncementCriteria
            {
                EmployeeId = SessionService.EmployeeId
            };
            var settings = baseService.GetData<JobLevelStandardProcessSetting>(criteria);
            foreach(var setting in settings)
            {
                var fromDate = setting.FromDate.Date.ToString(Constants.FormatDateTimeEmail);
                var toDate = setting.ToDate.AddDays(1).AddSeconds(-1).ToString(Constants.FormatDateTimeEmail);
                switch (setting.ProcessId)
                {
                    case (byte)JobLevelAssessmentProcess.Evaluator:
                        result.Add(string.Format("{0} Assessment is ready for key-in data. You are given a period from {1} until {2} to complete this Assessment.",
                            setting.PeriodName,
                            fromDate,
                            toDate));
                        break;

                    case (byte)JobLevelAssessmentProcess.Employee:
                        result.Add(string.Format("{0} Assessment is ready for your Verification. You are given a period from {1} until {2} to complete the Verification.",
                            setting.PeriodName,
                            fromDate,
                            toDate));
                        break;

                    case (byte)JobLevelAssessmentProcess.Management:
                        result.Add(string.Format("{0} Assessment is ready for your Approval. You are given a period from {1} until {2} to complete the Approval.",
                            setting.PeriodName,
                            fromDate,
                            toDate));
                        break;

                    default:
                        // nothing
                        break;
                }
            }

            return result;
        }
    }
}