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
    using System.Web.Http;
    using SMS.EPA.Criteria.Report;
    using SMS.EPA.Model.Report.Dashboard;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class DashboardController : BaseController
    {
        private readonly IReportService _reportService;
        private readonly CommonController _commonController;

        public DashboardController() :
            this(null, null)
        {
        }

        public DashboardController(
            IReportService reportService,
            CommonController commonController)
        {
            _reportService = reportService ?? new ReportService();
            _commonController = commonController ?? new CommonController();
        }

        [HttpGet]
        public DashboardModel GetDashboard()
        {
            DashboardModel result = new DashboardModel();

            var criteria = new GetDashboardCriteria()
            {
                EmployeeId = CurrentUser.EmployeeInfo.EmployeeId
            };

            result = _reportService.GetDashboard(criteria);
            result.Employee = _commonController.GetEmployeeById(CurrentUser.EmployeeInfo.EmployeeId);

            return result;
        }
    }
}