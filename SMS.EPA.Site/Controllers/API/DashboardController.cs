// -----------------------------------------------------------------------
// <copyright file="DashboardController.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.07">Created</change>
// </history>

namespace SMS.EPA.Site.Controllers.API
{
    using System.Linq;
    using System.Web.Http;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Criteria.Report;
    using SMS.EPA.Model.Report.Dashboard;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;
    using SMS.EPA.Web.ApiAuthorizeAttribute;

    public class DashboardController : BaseController
    {
        private readonly IReportService _reportService;

        public DashboardController() :
            this(null)
        {
        }

        public DashboardController(
            IReportService reportService)
        {
            _reportService = reportService ?? new ReportService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.Home, Policy = Policy.Read)]
        public DashboardModel GetDashboard()
        {
            DashboardModel dashboard = new DashboardModel();

            var criteria = new GetDashboardCriteria()
            {
                EmployeeId = CurrentUser.EmployeeInfo.EmployeeId
            };

            dashboard = _reportService.GetDashboard(criteria);
            dashboard.Employee = CurrentUser.EmployeeInfo;
            dashboard.MonthlyEPAs = dashboard.MyEPAs.Where(x => x.Period.TargetTypeId == (int)TargetType.Monthly);
            dashboard.MyEPAs = dashboard.MyEPAs.Where(x => x.Period.TargetTypeId != (int)TargetType.Monthly);

            return dashboard;
        }
    }
}