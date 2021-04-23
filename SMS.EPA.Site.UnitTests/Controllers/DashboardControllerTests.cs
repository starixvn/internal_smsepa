namespace SMS.EPA.Site.UnitTests
{
    using System.Collections.Generic;
    using Core.Services;
    using Criteria;
    using Model;
    using Model.Enitites;
    using NSubstitute;
    using NUnit.Framework;
    using Services.Interfaces;
    using Site.Controllers.API;

    [TestFixture]
    public class DashboardControllerTests
    {
        private IEmployeeService employeeService;
        private IReportService reportService;
        private IBaseService baseService;
        private ISessionService sessionService;
        private DashboardController dashboardController;

        [Test]
        public void GetDashboard_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            sessionService = FakeSessionService();
            reportService = FakeReportService();
            employeeService = FakeEmployeeService();
            reportService.GetDashboard(Arg.Any<int>()).Returns(new DashboardModel());
            employeeService.GetEmployeeById(Arg.Any<int>()).Returns(new Employee());
            dashboardController = GetDashboardController(employeeService, reportService, sessionService);

            // Act
            var actual = dashboardController.GetDashboard();

            // Assert
            Assert.IsInstanceOf(typeof(DashboardModel), actual);
        }

        [Test]
        public void GetToDoList_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.GetData<ToDoListModel>(Arg.Any<GetToDoListCriteria>()).Returns(new List<ToDoListModel>());
            dashboardController = GetDashboardController(null, null, sessionService, baseService);

            // Act
            var actual = dashboardController.GetToDoList();

            // Assert
            Assert.IsInstanceOf(typeof(ToDoListViewModel), actual);
        }

        #region Helpers

        private static ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private static IEmployeeService FakeEmployeeService()
        {
            var service = Substitute.For<IEmployeeService>();

            return service;
        }

        private static IReportService FakeReportService()
        {
            var service = Substitute.For<IReportService>();

            return service;
        }

        private static IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private DashboardController GetDashboardController(
            IEmployeeService employeeService = null,
            IReportService reportService = null,
            ISessionService sessionService = null,
            IBaseService baseService = null)
        {
            var controller = new DashboardController(employeeService, reportService, baseService) { SessionService = sessionService };

            return controller;
        }

        #endregion Helpers
    }
}