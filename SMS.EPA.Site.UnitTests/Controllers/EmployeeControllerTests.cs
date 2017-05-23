namespace SMS.EPA.Site.UnitTests
{
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Model.Enitites;
    using NSubstitute;
    using NUnit.Framework;
    using Services.Interfaces;
    using Site.Controllers.API;
    using System.Collections.Generic;

    [TestFixture]
    public class EmployeeControllerTests
    {
        private IEmployeeService _employeeService;
        private ISessionService _sessionService;
        private EmployeeController _employeeController;

        #region GetAllEmployee

        [Test]
        public void GetAllEmployee_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _employeeController = GetEmployeeController();

            // Act
            var actual = _employeeController.GetType().GetMethod("GetAllEmployee");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetAllEmployee_WhenCalled_ReturnInstanceOfListOfEmployeeModel()
        {
            // Arrange
            _employeeService = FakeEmployeeService();
            _employeeService.GetAllEmployee(Arg.Any<GetAllEmployeeCriteria>()).Returns(new List<Employee>());
            _employeeController = GetEmployeeController(_employeeService);

            // Act
            var actual = _employeeController.GetAllEmployee(Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<bool?>(), Arg.Any<bool?>(), Arg.Any<bool?>(), Arg.Any<bool?>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<Employee>), actual);
        }

        #endregion GetAllEmployee

        #region CreateSpecialEmployee

        [Test]
        public void CreateSpecialEmployee_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _employeeController = GetEmployeeController();

            // Act
            var actual = _employeeController.GetType().GetMethod("CreateSpecialEmployee");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void CreateSpecialEmployee_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _employeeService = FakeEmployeeService();
            _sessionService = FakeSessionService();
            _employeeService.CreateSpecialEmployee(Arg.Any<CreateSpecialEmployeeCriteria>()).Returns(new CommandResult());
            _employeeController = GetEmployeeController(_employeeService, _sessionService);

            // Act
            var actual = _employeeController.CreateSpecialEmployee(Arg.Any<CreateSpecialEmployeeCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion CreateSpecialEmployee

        #region UpdateSpecialEmployee

        [Test]
        public void UpdateSpecialEmployee_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _employeeController = GetEmployeeController();

            // Act
            var actual = _employeeController.GetType().GetMethod("UpdateSpecialEmployee");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void UpdateSpecialEmployee_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _employeeService = FakeEmployeeService();
            _sessionService = FakeSessionService();
            _employeeService.UpdateSpecialEmployee(Arg.Any<UpdateSpecialEmployeeCriteria>()).Returns(new CommandResult());
            _employeeController = GetEmployeeController(_employeeService, _sessionService);

            // Act
            var actual = _employeeController.UpdateSpecialEmployee(Arg.Any<UpdateSpecialEmployeeCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateSpecialEmployee

        #region AssignRoleToEmployee

        [Test]
        public void AssignRoleToEmployee_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _employeeController = GetEmployeeController();

            // Act
            var actual = _employeeController.GetType().GetMethod("AssignRoleToEmployee");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void AssignRoleToEmployee_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _employeeService = FakeEmployeeService();
            _sessionService = FakeSessionService();
            _employeeService.AssignRoleToEmployee(Arg.Any<AssignRoleToEmployeeCriteria>()).Returns(new CommandResult());
            _employeeController = GetEmployeeController(_employeeService, _sessionService);

            // Act
            var actual = _employeeController.AssignRoleToEmployee(Arg.Any<AssignRoleToEmployeeCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion AssignRoleToEmployee

        #region ChangePassword

        [Test]
        public void ChangePassword_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _employeeController = GetEmployeeController();

            // Act
            var actual = _employeeController.GetType().GetMethod("ChangePassword");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void ChangePassword_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _employeeService = FakeEmployeeService();
            _sessionService = FakeSessionService();
            _employeeService.ChangePassword(Arg.Any<ChangePasswordCriteria>()).Returns(new CommandResult());
            _employeeController = GetEmployeeController(_employeeService, _sessionService);

            // Act
            var actual = _employeeController.ChangePassword(Arg.Any<ChangePasswordCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion ChangePassword

        #region ResetPassword

        [Test]
        public void ResetPassword_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _employeeController = GetEmployeeController();

            // Act
            var actual = _employeeController.GetType().GetMethod("ResetPassword");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void ResetPassword_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _employeeService = FakeEmployeeService();
            _sessionService = FakeSessionService();
            ConfigurationManagerService.GetAppSetting(Arg.Any<string>()).Returns("123456");
            _employeeService.ResetPassword(Arg.Any<ResetPasswordCriteria>()).Returns(new CommandResult());
            _employeeController = GetEmployeeController(_employeeService, _sessionService);

            // Act
            var actual = _employeeController.ResetPassword(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion ResetPassword

        #region UpdateHasMonthlyEPA

        [Test]
        public void UpdateHasMonthlyEPA_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _employeeController = GetEmployeeController();

            // Act
            var actual = _employeeController.GetType().GetMethod("UpdateHasMonthlyEPA");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void UpdateHasMonthlyEPA_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _employeeService = FakeEmployeeService();
            _sessionService = FakeSessionService();
            _employeeService.UpdateHasMonthlyEPA(Arg.Any<UpdateHasMonthlyEPACriteria>()).Returns(new CommandResult());
            _employeeController = GetEmployeeController(_employeeService, _sessionService);

            // Act
            var actual = _employeeController.UpdateHasMonthlyEPA(Arg.Any<UpdateHasMonthlyEPACriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateHasMonthlyEPA

        #region Helpers

        private ISessionService FakeSessionService()
        {
            ISessionService service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private IEmployeeService FakeEmployeeService()
        {
            IEmployeeService service = Substitute.For<IEmployeeService>();

            return service;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Reviewed")]
        private EmployeeController GetEmployeeController(
            IEmployeeService employeeService = null,
            ISessionService sessionService = null)
        {
            var controller = new EmployeeController(employeeService);
            controller.SessionService = sessionService;

            return controller;
        }

        #endregion Helpers
    }
}