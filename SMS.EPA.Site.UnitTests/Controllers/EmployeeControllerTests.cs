namespace SMS.EPA.Site.UnitTests
{
    using System.Collections.Generic;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services.Interfaces;
    using SMS.EPA.Criteria;
    using SMS.EPA.Services.Interfaces;
    using SMS.EPA.Site.Controllers.API;
    using SMS.EPA.Model.Enitites;

    [TestFixture]
    public class EmployeeControllerTests
    {
        private IEmployeeService employeeService;
        private ISessionService sessionService;
        private EmployeeController employeeController;

        #region GetAllEmployee

        [Test]
        public void GetAllEmployee_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            employeeController = GetEmployeeController();

            // Act
            var actual = employeeController.GetType().GetMethod("GetAllEmployee");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetAllEmployee_WhenCalled_ReturnInstanceOfListOfEmployeeModel()
        {
            // Arrange
            employeeService = FakeEmployeeService();
            employeeService.GetAllEmployee(Arg.Any<GetAllEmployeeCriteria>()).Returns(new List<Employee>());
            employeeController = GetEmployeeController(employeeService);

            // Act
            var actual = employeeController.GetAllEmployee(Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<bool?>(), Arg.Any<bool?>(), Arg.Any<bool?>(), Arg.Any<bool?>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<Employee>), actual);
        }

        #endregion GetAllEmployee

        #region CreateSpecialEmployee

        [Test]
        public void CreateSpecialEmployee_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            employeeController = GetEmployeeController();

            // Act
            var actual = employeeController.GetType().GetMethod("CreateSpecialEmployee");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void CreateSpecialEmployee_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            employeeService = FakeEmployeeService();
            sessionService = FakeSessionService();
            employeeService.CreateSpecialEmployee(Arg.Any<CreateSpecialEmployeeCriteria>()).Returns(new CommandResult());
            employeeController = GetEmployeeController(employeeService, sessionService);

            // Act
            var actual = employeeController.CreateSpecialEmployee(Arg.Any<CreateSpecialEmployeeCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion CreateSpecialEmployee

        #region UpdateSpecialEmployee

        [Test]
        public void UpdateSpecialEmployee_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            employeeController = GetEmployeeController();

            // Act
            var actual = employeeController.GetType().GetMethod("UpdateSpecialEmployee");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void UpdateSpecialEmployee_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            employeeService = FakeEmployeeService();
            sessionService = FakeSessionService();
            employeeService.UpdateSpecialEmployee(Arg.Any<UpdateSpecialEmployeeCriteria>()).Returns(new CommandResult());
            employeeController = GetEmployeeController(employeeService, sessionService);

            // Act
            var actual = employeeController.UpdateSpecialEmployee(Arg.Any<UpdateSpecialEmployeeCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateSpecialEmployee

        #region AssignRoleToEmployee

        [Test]
        public void AssignRoleToEmployee_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            employeeController = GetEmployeeController();

            // Act
            var actual = employeeController.GetType().GetMethod("AssignRoleToEmployee");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void AssignRoleToEmployee_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            employeeService = FakeEmployeeService();
            sessionService = FakeSessionService();
            employeeService.AssignRoleToEmployee(Arg.Any<AssignRoleToEmployeeCriteria>()).Returns(new CommandResult());
            employeeController = GetEmployeeController(employeeService, sessionService);

            // Act
            var actual = employeeController.AssignRoleToEmployee(Arg.Any<AssignRoleToEmployeeCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion AssignRoleToEmployee

        #region ChangePassword

        [Test]
        public void ChangePassword_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            employeeController = GetEmployeeController();

            // Act
            var actual = employeeController.GetType().GetMethod("ChangePassword");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void ChangePassword_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            employeeService = FakeEmployeeService();
            sessionService = FakeSessionService();
            employeeService.ChangePassword(Arg.Any<ChangePasswordCriteria>()).Returns(new CommandResult());
            employeeController = GetEmployeeController(employeeService, sessionService);

            // Act
            var actual = employeeController.ChangePassword(Arg.Any<ChangePasswordCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion ChangePassword

        #region ResetPassword

        [Test]
        public void ResetPassword_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            employeeController = GetEmployeeController();

            // Act
            var actual = employeeController.GetType().GetMethod("ResetPassword");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void ResetPassword_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            employeeService = FakeEmployeeService();
            sessionService = FakeSessionService();
            string oldPassword = "123456";
            employeeService.GetPasswordByEmployeeId(Arg.Any<int>()).Returns(oldPassword);
            employeeService.ChangePassword(Arg.Any<ChangePasswordCriteria>()).Returns(new CommandResult());
            employeeController = GetEmployeeController(employeeService, sessionService);

            // Act
            var actual = employeeController.ResetPassword(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion ResetPassword

        #region UpdateHasMonthlyEPA

        [Test]
        public void UpdateHasMonthlyEPA_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            employeeController = GetEmployeeController();

            // Act
            var actual = employeeController.GetType().GetMethod("UpdateHasMonthlyEPA");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void UpdateHasMonthlyEPA_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            employeeService = FakeEmployeeService();
            sessionService = FakeSessionService();
            employeeService.UpdateHasMonthlyEPA(Arg.Any<UpdateHasMonthlyEPACriteria>()).Returns(new CommandResult());
            employeeController = GetEmployeeController(employeeService, sessionService);

            // Act
            var actual = employeeController.UpdateHasMonthlyEPA(Arg.Any<UpdateHasMonthlyEPACriteria>());

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