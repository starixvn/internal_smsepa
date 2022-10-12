namespace SMS.EPA.Site.UnitTests
{
    using System.Collections.Generic;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Fanex.Data.Repository;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using Model.Enitites;
    using NSubstitute;
    using NUnit.Framework;
    using Site.Controllers.API;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Services;

    [TestFixture]
    public class EmployeeControllerTests
    {
        private IEmployeeService employeeService;
        private IBaseService baseService;
        private IUserControlSession userControlSession;
        private ICryptoService cryptoService;
        private IUserService userService;

        #region GetAllEmployee

        [Test]
        public void GetAllEmployee_WhenCalled_ReturnExpectedEmployees()
        {
            // Arrange
            var expected = new List<Employee>();
            var controller = GetEmployeeController();
            employeeService.GetEmployee(Arg.Any<GetEmployeeCriteria>()).Returns(expected);

            // Act
            var actual = controller.GetAllEmployee(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool?>(), Arg.Any<bool?>(), Arg.Any<bool?>(), Arg.Any<bool?>());

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
            var controller = GetEmployeeController();
            employeeService.CreateSpecialEmployee(Arg.Any<CreateSpecialEmployeeCriteria>()).Returns(expected);

            // Act
            var actual = controller.CreateSpecialEmployee(Arg.Any<CreateSpecialEmployeeCriteria>());

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
            var controller = GetEmployeeController();
            baseService.ExecuteNonQuery(Arg.Any<UpdateSpecialEmployeeCriteria>()).Returns(expected);

            // Act
            var actual = controller.UpdateSpecialEmployee(Arg.Any<UpdateSpecialEmployeeCriteria>());

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
            var controller = GetEmployeeController();
            baseService.ExecuteNonQuery(Arg.Any<ICriteria>()).Returns(expected);

            // Act
            var actual = controller.AssignRoleToEmployee(Arg.Any<AssignRoleToEmployeeCriteria>());

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
            var controller = GetEmployeeController();
            employeeService.ChangePassword(Arg.Any<ChangePasswordCriteria>()).Returns(expected);

            // Act
            var actual = controller.ChangePassword(Arg.Any<ChangePasswordCriteria>());

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
            var controller = GetEmployeeController();
            baseService.ExecuteNonQuery(Arg.Any<UpdateHasMonthlyEPACriteria>()).Returns(expected);

            // Act
            var actual = controller.UpdateHasMonthlyEPA(Arg.Any<UpdateHasMonthlyEPACriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion UpdateHasMonthlyEPA

        #region Helpers

        private static IUserControlSession FakeUserControlSession()
        {
            var session = Substitute.For<IUserControlSession>();
            session.Set<int>(Constants.CurrentEmployeeIdKey, 1);

            return session;
        }

        private EmployeeController GetEmployeeController()
        {
            userControlSession = FakeUserControlSession();
            employeeService = Substitute.For<IEmployeeService>();
            baseService = Substitute.For<IBaseService>();
            cryptoService = Substitute.For<ICryptoService>();
            userService = Substitute.For<IUserService>();
            var controller = new EmployeeController(employeeService, baseService, cryptoService, userService)
            {
                UserControlSession = userControlSession
            };

            return controller;
        }

        #endregion Helpers
    }
}