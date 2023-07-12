namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models.Role;
    using HRM.UserControl.Client.Core.Services;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Criteria.Dashboard;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Dashboard;
    using SMS.EPA.Services;

    public class DashboardController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly IReportService reportService;
        private readonly IBaseService baseService;
        private readonly IRoleService roleService;

        public DashboardController() : this(null, null, null, null)
        {
        }

        public DashboardController(
            IBaseService baseService,
            IEmployeeService employeeService,
            IReportService reportService,
            IRoleService roleService)
        {
            this.baseService = baseService ?? new BaseService();
            this.employeeService = employeeService ?? new EmployeeService();
            this.reportService = reportService ?? new ReportService();
            this.roleService = roleService ?? new RoleService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Home, Policy = Policy.Read)]
        public DashboardModel GetDashboard()
        {
            DashboardModel dashboard = reportService.GetDashboard(CurrentEmployeeId);
            IList<Role> roles = roleService.GetBySiteId(UserControlSession.SiteId);
            dashboard.Employee = employeeService.GetEmployeeById(CurrentEmployeeId, roles);

            return dashboard;
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Home, Policy = Policy.Read)]
        public ToDoListViewModel GetToDoList()
        {
            return new ToDoListViewModel
            {
                EPAToDoList = GetEPAToDoList(),
                JLSToDoList = GetJobLevelStandardToDoList()
            };
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Home, Policy = Policy.Read)]
        public IList<object> GetAnnouncement()
        {
            var result = new List<object>();
            var criteria = new GetAnnouncementCriteria
            {
                EmployeeId = CurrentEmployeeId
            };
            var settings = baseService.GetData<JobLevelStandardProcessSetting>(criteria);
            foreach (var setting in settings)
            {
                var fromDate = setting.FromDate.Date.ToString(Constants.FormatDateTimeEmail);
                var toDate = setting.ToDate.AddDays(1).AddSeconds(-1).ToString(Constants.FormatDateTimeEmail);
                switch (setting.ProcessId)
                {
                    case (byte)JobLevelAssessmentProcess.Evaluator:
                        result.Add(new
                        {
                            Announcement = string.Format("{0} Assessment is ready for key-in data. You are given a period from {1} until {2} to complete this Assessment.",
                            setting.PeriodName,
                            fromDate,
                            toDate),
                            PeriodId = setting.PeriodId,
                        });
                        break;

                    case (byte)JobLevelAssessmentProcess.Employee:
                        result.Add(new
                        {
                            Announcement = string.Format("{0} Assessment is ready for your Verification. You are given a period from {1} until {2} to complete the Verification.",
                            setting.PeriodName,
                            fromDate,
                            toDate),
                            PeriodId = setting.PeriodId,
                        });
                        break;

                    case (byte)JobLevelAssessmentProcess.Management:
                        result.Add(new
                        {
                            Announcement = string.Format("{0} Assessment is ready for your Approval. You are given a period from {1} until {2} to complete the Approval.",
                            setting.PeriodName,
                            fromDate,
                            toDate),
                            PeriodId = setting.PeriodId,
                        });
                        break;

                    default:
                        // nothing
                        break;
                }
            }

            return result;
        }

        private EPAToDoListViewModel GetEPAToDoList()
        {
            var criteria = new GetToDoListCriteria
            {
                EmployeeId = CurrentEmployeeId
            };
            return new EPAToDoListViewModel
            {
                ToDoList = baseService.GetData<EPAToDoList>(criteria)
            };
        }

        private JobLevelStandardDashboardViewModel GetJobLevelStandardToDoList()
        {
            var criteria = new GetJobLevelStandardDashboardCriteria
            {
                EmployeeId = CurrentEmployeeId
            };
            return new JobLevelStandardDashboardViewModel
            {
                ToDoList = baseService.GetData<JobLevelStandardDashboard>(criteria)
            };
        }
    }
}