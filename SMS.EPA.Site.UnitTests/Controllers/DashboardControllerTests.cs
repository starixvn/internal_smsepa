namespace SMS.EPA.Site.UnitTests
{
    using System.Collections.Generic;
    using Core.Services;
    using Criteria;
    using HRM.UserControl.Client.Core.Session;
    using Model;
    using Model.Enitites;
    using NSubstitute;
    using NUnit.Framework;
    using Services;
    using Site.Controllers.API;
    using SMS.EPA.Core.Constants;

    [TestFixture]
    public class DashboardControllerTests
    {
        private IEmployeeService employeeService;
        private IReportService reportService;
        private IBaseService baseService;
        private IUserControlSession userControlSession;

        [Test]
        public void GetDashboard_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var controller = GetDashboardController();
            reportService.GetDashboard(Arg.Any<int>()).Returns(new DashboardModel());
            employeeService.GetEmployeeById(Arg.Any<int>()).Returns(new Employee());

            // Act
            var actual = controller.GetDashboard();

            // Assert
            Assert.IsInstanceOf(typeof(DashboardModel), actual);
        }

        [Test]
        public void GetToDoList_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var controller = GetDashboardController();
            baseService.GetData<ToDoListModel>(Arg.Any<GetToDoListCriteria>()).Returns(new List<ToDoListModel>());

            // Act
            var actual = controller.GetToDoList();

            // Assert
            Assert.IsInstanceOf(typeof(ToDoListViewModel), actual);
        }

        #region Helpers

        private static IUserControlSession FakeUserControlSession()
        {
            var session = Substitute.For<IUserControlSession>();
            session.Set<int>(Constants.CurrentEmployeeIdKey, 1);

            return session;
        }

        private DashboardController GetDashboardController()
        {
            userControlSession = FakeUserControlSession();
            reportService = Substitute.For<IReportService>();
            employeeService = Substitute.For<IEmployeeService>();
            baseService = Substitute.For<IBaseService>();
            var controller = new DashboardController(employeeService, reportService, baseService)
            {
                UserControlSession = userControlSession
            };

            return controller;
        }

        #endregion Helpers
    }
}