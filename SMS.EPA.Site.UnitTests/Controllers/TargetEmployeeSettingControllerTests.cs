namespace SMS.EPA.Site.UnitTests.Controllers
{
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.TargetEmployeeSetting;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Interfaces;
    using SMS.EPA.Site.Controllers.API;
    using System.Collections.Generic;

    [TestFixture]
    public class TargetEmployeeSettingControllerTests
    {
        private ITargetEmployeeSettingService targetEmployeeSettingService;
        private TargetEmployeeSettingController targetEmployeeSettingController;

        #region GetTargetEmployeeSetting

        [Test]
        public void GetTargetEmployeeSetting_AnyArg_ReturnExpectedTargetEmployees()
        {
            // Arrange
            var expectedTargetEmployees = new List<TargetEmployeeSetting>();
            expectedTargetEmployees.Add(new TargetEmployeeSetting());
            ArrangeGetTargetEmployeeSetting(expectedTargetEmployees);

            // Act
            var actual = targetEmployeeSettingController.GetTargetEmployeeSetting();

            // Assert
            Assert.AreEqual(expectedTargetEmployees, actual);
        }

        [Test]
        public void GetTargetEmployeeSetting_AnyArg_ReturnListOfTargetEmployeeSetting()
        {
            // Arrange
            ArrangeGetTargetEmployeeSetting();

            // Act
            var actual = targetEmployeeSettingController.GetTargetEmployeeSetting();

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<TargetEmployeeSetting>), actual);
        }

        #endregion

        #region GetTargetDepartmentSetting

        [Test]
        public void GetTargetDepartmentSetting_AnyArg_ReturnExpectedTargetDepartments()
        {
            // Arrange
            var expectedTargetDepartments = new List<TargetDepartmentSetting>();
            expectedTargetDepartments.Add(new TargetDepartmentSetting());
            ArrangeGetTargetDepartmentSetting(expectedTargetDepartments);

            // Act
            var actual = targetEmployeeSettingController.GetTargetDepartmentSetting();

            // Assert
            Assert.AreEqual(expectedTargetDepartments, actual);
        }

        [Test]
        public void GetTargetDepartmentSetting_AnyArg_ReturnListOfTargetDepartmentSetting()
        {
            // Arrange
            ArrangeGetTargetDepartmentSetting();

            // Act
            var actual = targetEmployeeSettingController.GetTargetEmployeeSetting();

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<TargetEmployeeSetting>), actual);
        }

        #endregion

        #region AssignTargetTemplateToEmployee

        [Test]
        public void AssignTargetTemplateToEmployee_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeAssignTargetTemplateToEmployee();

            // Act
            var actual = targetEmployeeSettingController.AssignTargetTemplateToEmployee(Arg.Any<AssignTargetTemplateToEmployeeCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void AssignTargetTemplateToEmployee_AnyArg_ReturnExpectedCommandResult()
        {
            // Arrange
            var expectedCommandResult = new CommandResult() 
            { 
                Code = 1
            };
            ArrangeAssignTargetTemplateToEmployee(expectedCommandResult);

            // Act
            var actual = targetEmployeeSettingController.AssignTargetTemplateToEmployee(new AssignTargetTemplateToEmployeeCriteria());

            // Assert
            Assert.AreEqual(expectedCommandResult, actual);
        }

        #endregion

        #region DeleteTargetEmployeeSetting

        [Test]
        public void DeleteTargetEmployeeSetting_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeDeleteTargetEmployeeSetting();

            // Act
            var actual = targetEmployeeSettingController.DeleteTargetEmployeeSetting(Arg.Any<int>(), Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void DeleteTargetEmployeeSetting_AnyArg_ReturnExpectedCommandResult()
        {
            // Arrange
            var expectedCommandResult = new CommandResult()
            {
                Code = 1
            };
            ArrangeDeleteTargetEmployeeSetting(expectedCommandResult);

            // Act
            var actual = targetEmployeeSettingController.DeleteTargetEmployeeSetting(Arg.Any<int>(), Arg.Any<int>());

            // Assert
            Assert.AreEqual(expectedCommandResult, actual);
        }

        #endregion

        #region AssignTargetTemplateToDepartment

        [Test]
        public void AssignTargetTemplateToDepartment_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeAssignTargetTemplateToDepartment();

            // Act
            var actual = targetEmployeeSettingController.AssignTargetTemplateToDepartment(null);

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void AssignTargetTemplateToDepartment_ModelIsNotNull_ReturnExpectedCommandResult()
        {
            // Arrange
            var expectedCommandResult = new CommandResult()
            {
                Code = 1
            };
            ArrangeAssignTargetTemplateToDepartment(expectedCommandResult);

            // Act
            var actual = targetEmployeeSettingController.AssignTargetTemplateToDepartment(new TargetSettingAssignModel());

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
            var actual = targetEmployeeSettingController.AssignTargetTemplateToDepartment(null);

            // Assert
            Assert.AreEqual(expectedCommandResult.Code, actual.Code);
        }

        #endregion

        #region DeleteTargetDepartmentSetting

        [Test]
        public void DeleteTargetDepartmentSetting_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeDeleteTargetDepartmentSetting();

            // Act
            var actual = targetEmployeeSettingController.DeleteTargetDepartmentSetting(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void DeleteTargetDepartmentSetting_AnyArg_ReturnExpectedCommandResult()
        {
            // Arrange
            var expectedCommandResult = new CommandResult()
            {
                Code = 1
            };
            ArrangeDeleteTargetDepartmentSetting(expectedCommandResult);

            // Act
            var actual = targetEmployeeSettingController.DeleteTargetDepartmentSetting(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());

            // Assert
            Assert.AreEqual(expectedCommandResult, actual);
        }

        #endregion

        #region GetUserPermissionOnTargetEmployeeSetting

        [Test]
        public void GetUserPermissionOnTargetEmployeeSetting_AnyArg_ReturnExpectedPermission()
        {
            // Arrange
            var expectedPermission = 2;
            targetEmployeeSettingController = new TargetEmployeeSettingController();
            targetEmployeeSettingController.SessionService = FakeSessionService(expectedPermission);

            // Act
            var actual = targetEmployeeSettingController.GetUserPermissionOnTargetEmployeeSetting();

            // Assert
            Assert.AreEqual(expectedPermission, actual);
        }

        #endregion

        private ISessionService FakeSessionService(int? permission = null)
        {
            ISessionService service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;
            service.GetPermission(Arg.Any<string>()).Returns(permission.HasValue ? permission.Value : 0);

            return service;
        }

        private void ArrangeGetTargetEmployeeSetting(IList<TargetEmployeeSetting> targetEmployees = null)
        {
            targetEmployeeSettingService = Substitute.For<ITargetEmployeeSettingService>();

            targetEmployeeSettingService.GetTargetEmployeeSetting(Arg.Any<GetAllTargetEmployeeSettingCriteria>())
                .Returns(targetEmployees ?? new List<TargetEmployeeSetting>());
            targetEmployeeSettingController = new TargetEmployeeSettingController(targetEmployeeSettingService);
        }

        private void ArrangeGetTargetDepartmentSetting(IList<TargetDepartmentSetting> targetDepartments = null)
        {
            targetEmployeeSettingService = Substitute.For<ITargetEmployeeSettingService>();

            targetEmployeeSettingService.GetTargetDepartmentSetting(Arg.Any<GetTargetDepartmentSettingCriteria>())
                .Returns(targetDepartments ?? new List<TargetDepartmentSetting>());
            targetEmployeeSettingController = new TargetEmployeeSettingController(targetEmployeeSettingService);
        }

        private void ArrangeAssignTargetTemplateToEmployee(CommandResult result = null)
        {
            targetEmployeeSettingService = Substitute.For<ITargetEmployeeSettingService>();

            targetEmployeeSettingService.AssignTargetTemplateToEmployee(Arg.Any<AssignTargetTemplateToEmployeeCriteria>()).Returns(result ?? new CommandResult());
            targetEmployeeSettingController = new TargetEmployeeSettingController(targetEmployeeSettingService);
            targetEmployeeSettingController.SessionService = FakeSessionService();
        }

        private void ArrangeDeleteTargetEmployeeSetting(CommandResult result = null)
        {
            targetEmployeeSettingService = Substitute.For<ITargetEmployeeSettingService>();

            targetEmployeeSettingService.DeleteTargetEmployeeSetting(Arg.Any<DeleteTargetEmployeeSettingCriteria>()).Returns(result ?? new CommandResult());
            targetEmployeeSettingController = new TargetEmployeeSettingController(targetEmployeeSettingService);
            targetEmployeeSettingController.SessionService = FakeSessionService();
        }

        private void ArrangeAssignTargetTemplateToDepartment(CommandResult result = null)
        {
            targetEmployeeSettingService = Substitute.For<ITargetEmployeeSettingService>();

            targetEmployeeSettingService.AssignTargetTemplateToDepartment(Arg.Any<AssignTargetDepartmentSettingCriteria>()).Returns(result ?? new CommandResult());
            targetEmployeeSettingController = new TargetEmployeeSettingController(targetEmployeeSettingService);
            targetEmployeeSettingController.SessionService = FakeSessionService();
        }

        private void ArrangeDeleteTargetDepartmentSetting(CommandResult result = null)
        {
            targetEmployeeSettingService = Substitute.For<ITargetEmployeeSettingService>();

            targetEmployeeSettingService.DeleteTargetDepartmentSetting(Arg.Any<DeleteTargetDepartmentSettingCriteria>()).Returns(result ?? new CommandResult());
            targetEmployeeSettingController = new TargetEmployeeSettingController(targetEmployeeSettingService);
            targetEmployeeSettingController.SessionService = FakeSessionService();
        }
    }
}
