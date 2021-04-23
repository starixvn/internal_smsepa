namespace SMS.EPA.Site.UnitTests.Controllers
{
    using System.Collections.Generic;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Model;
    using NSubstitute;
    using NUnit.Framework;
    using Services.Interfaces;
    using Site.Controllers.API;

    [TestFixture]
    public class TargetEmployeeSettingControllerTests
    {
        private ITargetEmployeeSettingService _targetEmployeeSettingService;
        private IBaseService baseService;
        private TargetEmployeeSettingController _targetEmployeeSettingController;

        #region GetTargetEmployeeSetting

        [Test]
        public void GetTargetEmployeeSetting_AnyArg_ReturnExpectedTargetEmployees()
        {
            // Arrange
            var expectedTargetEmployees = new List<TargetEmployeeSetting> { new TargetEmployeeSetting() };
            ArrangeGetTargetEmployeeSetting(expectedTargetEmployees);

            // Act
            var actual = _targetEmployeeSettingController.GetTargetEmployeeSetting();

            // Assert
            Assert.AreEqual(expectedTargetEmployees, actual);
        }

        [Test]
        public void GetTargetEmployeeSetting_AnyArg_ReturnListOfTargetEmployeeSetting()
        {
            // Arrange
            ArrangeGetTargetEmployeeSetting();

            // Act
            var actual = _targetEmployeeSettingController.GetTargetEmployeeSetting();

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<TargetEmployeeSetting>), actual);
        }

        #endregion GetTargetEmployeeSetting

        #region GetTargetDepartmentSetting

        [Test]
        public void GetTargetDepartmentSetting_AnyArg_ReturnExpectedTargetDepartments()
        {
            // Arrange
            var expectedTargetDepartments = new List<TargetDepartmentSetting> { new TargetDepartmentSetting() };
            ArrangeGetTargetDepartmentSetting(expectedTargetDepartments);

            // Act
            var actual = _targetEmployeeSettingController.GetTargetDepartmentSetting();

            // Assert
            Assert.AreEqual(expectedTargetDepartments, actual);
        }

        [Test]
        public void GetTargetDepartmentSetting_AnyArg_ReturnListOfTargetDepartmentSetting()
        {
            // Arrange
            ArrangeGetTargetDepartmentSetting();

            // Act
            var actual = _targetEmployeeSettingController.GetTargetEmployeeSetting();

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<TargetEmployeeSetting>), actual);
        }

        #endregion GetTargetDepartmentSetting

        #region AssignTargetTemplateToEmployee

        [Test]
        public void AssignTargetTemplateToEmployee_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeAssignTargetTemplateToEmployee();

            // Act
            var actual = _targetEmployeeSettingController.AssignTargetTemplateToEmployee(Arg.Any<AssignTargetTemplateToEmployeeCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void AssignTargetTemplateToEmployee_AnyArg_ReturnExpectedCommandResult()
        {
            // Arrange
            var expectedCommandResult = new CommandResult
            {
                Code = 1
            };
            ArrangeAssignTargetTemplateToEmployee(expectedCommandResult);

            // Act
            var actual = _targetEmployeeSettingController.AssignTargetTemplateToEmployee(new AssignTargetTemplateToEmployeeCriteria());

            // Assert
            Assert.AreEqual(expectedCommandResult, actual);
        }

        #endregion AssignTargetTemplateToEmployee

        #region DeleteTargetEmployeeSetting

        [Test]
        public void DeleteTargetEmployeeSetting_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeDeleteTargetEmployeeSetting();

            // Act
            var actual = _targetEmployeeSettingController.DeleteTargetEmployeeSetting(Arg.Any<int>(), Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void DeleteTargetEmployeeSetting_AnyArg_ReturnExpectedCommandResult()
        {
            // Arrange
            var expectedCommandResult = new CommandResult
            {
                Code = 1
            };
            ArrangeDeleteTargetEmployeeSetting(expectedCommandResult);

            // Act
            var actual = _targetEmployeeSettingController.DeleteTargetEmployeeSetting(Arg.Any<int>(), Arg.Any<int>());

            // Assert
            Assert.AreEqual(expectedCommandResult, actual);
        }

        #endregion DeleteTargetEmployeeSetting

        #region AssignTargetTemplateToDepartment

        [Test]
        public void AssignTargetTemplateToDepartment_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeAssignTargetTemplateToDepartment();

            // Act
            var actual = _targetEmployeeSettingController.AssignTargetTemplateToDepartment(null);

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void AssignTargetTemplateToDepartment_ModelIsNotNull_ReturnExpectedCommandResult()
        {
            // Arrange
            var expectedCommandResult = new CommandResult
            {
                Code = 1
            };
            ArrangeAssignTargetTemplateToDepartment(expectedCommandResult);

            // Act
            var actual = _targetEmployeeSettingController.AssignTargetTemplateToDepartment(new TargetSettingAssignModel());

            // Assert
            Assert.AreEqual(expectedCommandResult, actual);
        }

        [Test]
        public void AssignTargetTemplateToDepartment_ModelIsNull_ReturnDefaultCommandResult()
        {
            // Arrange
            var expectedCommandResult = new CommandResult();
            ArrangeAssignTargetTemplateToDepartment();

            // Act
            var actual = _targetEmployeeSettingController.AssignTargetTemplateToDepartment(null);

            // Assert
            Assert.AreEqual(expectedCommandResult.Code, actual.Code);
        }

        #endregion AssignTargetTemplateToDepartment

        #region DeleteTargetDepartmentSetting

        [Test]
        public void DeleteTargetDepartmentSetting_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeDeleteTargetDepartmentSetting();

            // Act
            var actual = _targetEmployeeSettingController.DeleteTargetDepartmentSetting(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void DeleteTargetDepartmentSetting_AnyArg_ReturnExpectedCommandResult()
        {
            // Arrange
            var expectedCommandResult = new CommandResult
            {
                Code = 1
            };
            ArrangeDeleteTargetDepartmentSetting(expectedCommandResult);

            // Act
            var actual = _targetEmployeeSettingController.DeleteTargetDepartmentSetting(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());

            // Assert
            Assert.AreEqual(expectedCommandResult, actual);
        }

        #endregion DeleteTargetDepartmentSetting

        #region GetUserPermissionOnTargetEmployeeSetting

        [Test]
        public void GetUserPermissionOnTargetEmployeeSetting_AnyArg_ReturnExpectedPermission()
        {
            // Arrange
            var expectedPermission = 2;
            _targetEmployeeSettingController = new TargetEmployeeSettingController { SessionService = FakeSessionService(expectedPermission) };

            // Act
            var actual = _targetEmployeeSettingController.GetUserPermissionOnTargetEmployeeSetting();

            // Assert
            Assert.AreEqual(expectedPermission, actual);
        }

        #endregion GetUserPermissionOnTargetEmployeeSetting

        private static ISessionService FakeSessionService(int? permission = null)
        {
            ISessionService service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;
            service.GetPermission(Arg.Any<string>()).Returns(permission ?? 0);

            return service;
        }

        private static IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private TargetEmployeeSettingController GetTargetEmployeeSettingController(
            ITargetEmployeeSettingService targetEmployeeSettingService = null,
            ISessionService sessionService = null,
            IBaseService baseService = null)
        {
            var controller =
                new TargetEmployeeSettingController(targetEmployeeSettingService, baseService)
                {
                    SessionService = sessionService
                };

            return controller;
        }

        private void ArrangeGetTargetEmployeeSetting(IList<TargetEmployeeSetting> targetEmployees = null)
        {
            _targetEmployeeSettingService = Substitute.For<ITargetEmployeeSettingService>();

            _targetEmployeeSettingService.GetTargetEmployeeSetting(Arg.Any<GetAllTargetEmployeeSettingCriteria>())
                .Returns(targetEmployees ?? new List<TargetEmployeeSetting>());
            _targetEmployeeSettingController = GetTargetEmployeeSettingController(_targetEmployeeSettingService);
        }

        private void ArrangeGetTargetDepartmentSetting(IList<TargetDepartmentSetting> targetDepartments = null)
        {
            _targetEmployeeSettingService = Substitute.For<ITargetEmployeeSettingService>();

            _targetEmployeeSettingService.GetTargetDepartmentSetting(Arg.Any<GetTargetDepartmentSettingCriteria>())
                .Returns(targetDepartments ?? new List<TargetDepartmentSetting>());
            _targetEmployeeSettingController = GetTargetEmployeeSettingController(_targetEmployeeSettingService);
        }

        private void ArrangeAssignTargetTemplateToEmployee(CommandResult result = null)
        {
            _targetEmployeeSettingService = Substitute.For<ITargetEmployeeSettingService>();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<AssignTargetTemplateToEmployeeCriteria>()).Returns(result ?? new CommandResult());
            _targetEmployeeSettingController = GetTargetEmployeeSettingController(_targetEmployeeSettingService, FakeSessionService(), baseService);
        }

        private void ArrangeDeleteTargetEmployeeSetting(CommandResult result = null)
        {
            _targetEmployeeSettingService = Substitute.For<ITargetEmployeeSettingService>();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<DeleteTargetEmployeeSettingCriteria>()).Returns(result ?? new CommandResult());
            _targetEmployeeSettingController = GetTargetEmployeeSettingController(_targetEmployeeSettingService, FakeSessionService(), baseService);
        }

        private void ArrangeAssignTargetTemplateToDepartment(CommandResult result = null)
        {
            _targetEmployeeSettingService = Substitute.For<ITargetEmployeeSettingService>();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<AssignTargetDepartmentSettingCriteria>()).Returns(result ?? new CommandResult());
            _targetEmployeeSettingController = GetTargetEmployeeSettingController(_targetEmployeeSettingService, FakeSessionService(), baseService);
        }

        private void ArrangeDeleteTargetDepartmentSetting(CommandResult result = null)
        {
            _targetEmployeeSettingService = Substitute.For<ITargetEmployeeSettingService>();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<DeleteTargetDepartmentSettingCriteria>()).Returns(result ?? new CommandResult());
            _targetEmployeeSettingController = GetTargetEmployeeSettingController(_targetEmployeeSettingService, FakeSessionService(), baseService);
        }
    }
}