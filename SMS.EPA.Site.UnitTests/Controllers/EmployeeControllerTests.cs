namespace SMS.EPA.Site.UnitTests
{
    using System.Collections.Generic;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Fanex.Data.Repository;
    using Model.Enitites;
    using NSubstitute;
    using NUnit.Framework;
    using Services.Interfaces;
    using Site.Controllers.API;

    [TestFixture]
    public class EmployeeControllerTests
    {
        private IEmployeeService _employeeService;
        private IBaseService baseService;
        private ISessionService _sessionService;
        private EmployeeController _employeeController;

        #region GetAllEmployee

        [Test]
        public void GetAllEmployee_WhenCalled_ReturnExpectedEmployees()
        {
            // Arrange
            var expected = new List<Employee>();
            _employeeService = FakeEmployeeService();
            _employeeService.GetAllEmployee(Arg.Any<GetAllEmployeeCriteria>()).Returns(expected);
            _employeeController = GetEmployeeController(_employeeService);

            // Act
            var actual = _employeeController.GetAllEmployee(Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<bool?>(), Arg.Any<bool?>(), Arg.Any<bool?>(), Arg.Any<bool?>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<Employee>), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion GetAllEmployee

        #region CreateSpecialEmployee

        [Test]
        public void CreateSpecialEmployee_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            _employeeService = FakeEmployeeService();
            _sessionService = FakeSessionService();
            _employeeService.CreateSpecialEmployee(Arg.Any<CreateSpecialEmployeeCriteria>()).Returns(expected);
            _employeeController = GetEmployeeController(_employeeService, _sessionService);

            // Act
            var actual = _employeeController.CreateSpecialEmployee(Arg.Any<CreateSpecialEmployeeCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion CreateSpecialEmployee

        #region UpdateSpecialEmployee

        [Test]
        public void UpdateSpecialEmployee_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            _employeeService = FakeEmployeeService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<UpdateSpecialEmployeeCriteria>()).Returns(expected);
            _employeeController = GetEmployeeController(_employeeService, _sessionService, baseService);

            // Act
            var actual = _employeeController.UpdateSpecialEmployee(Arg.Any<UpdateSpecialEmployeeCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion UpdateSpecialEmployee

        #region AssignRoleToEmployee

        [Test]
        public void AssignRoleToEmployee_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            _employeeService = FakeEmployeeService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<ICriteria>()).Returns(expected);
            _employeeController = GetEmployeeController(_employeeService, _sessionService, baseService);

            // Act
            var actual = _employeeController.AssignRoleToEmployee(Arg.Any<AssignRoleToEmployeeCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion AssignRoleToEmployee

        #region ChangePassword

        [Test]
        public void ChangePassword_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            _employeeService = FakeEmployeeService();
            _sessionService = FakeSessionService();
            _employeeService.ChangePassword(Arg.Any<ChangePasswordCriteria>()).Returns(expected);
            _employeeController = GetEmployeeController(_employeeService, _sessionService);

            // Act
            var actual = _employeeController.ChangePassword(Arg.Any<ChangePasswordCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion ChangePassword

        #region UpdateHasMonthlyEPA

        [Test]
        public void UpdateHasMonthlyEPA_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            _employeeService = FakeEmployeeService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<UpdateHasMonthlyEPACriteria>()).Returns(expected);
            _employeeController = GetEmployeeController(_employeeService, _sessionService, baseService);

            // Act
            var actual = _employeeController.UpdateHasMonthlyEPA(Arg.Any<UpdateHasMonthlyEPACriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion UpdateHasMonthlyEPA

        #region Helpers

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private IEmployeeService FakeEmployeeService()
        {
            var service = Substitute.For<IEmployeeService>();

            return service;
        }

        private IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private EmployeeController GetEmployeeController(
            IEmployeeService employeeService = null,
            ISessionService sessionService = null,
            IBaseService baseService = null)
        {
            var controller = new EmployeeController(employeeService, baseService) { SessionService = sessionService };

            return controller;
        }

        #endregion Helpers
    }
}