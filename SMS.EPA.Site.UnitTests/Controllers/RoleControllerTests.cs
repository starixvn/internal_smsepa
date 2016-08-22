namespace SMS.EPA.Site.UnitTests
{
    using System.Collections.Generic;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services.Interfaces;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Interfaces;
    using SMS.EPA.Site.Controllers.API;

    [TestFixture]
    public class RoleControllerTests
    {
        private IRoleService roleService;
        private ISessionService sessionService;
        private RoleController roleController;

        #region GetRoleFunctionByRoleId

        [Test]
        public void GetRoleFunctionByRoleId_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            roleController = GetRoleController();

            // Act
            var actual = roleController.GetType().GetMethod("GetRoleFunctionByRoleId");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetRoleFunctionByRoleId_WhenCalled_ReturnInstanceOfListOfRoleFunctionModel()
        {
            // Arrange
            roleService = FakeRoleService();
            sessionService = FakeSessionService();
            roleService.GetFunctionByRoleId(Arg.Any<int>()).Returns(new List<RoleFunctionModel>());
            roleController = GetRoleController(roleService, sessionService);

            // Act
            var actual = roleController.GetRoleFunctionByRoleId(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<RoleFunctionModel>), actual);
        }

        #endregion GetRoleFunctionByRoleId

        #region AssignFunctionToRole

        [Test]
        public void AssignFunctionToRole_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            roleController = GetRoleController();

            // Act
            var actual = roleController.GetType().GetMethod("AssignFunctionToRole");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void AssignFunctionToRole_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            roleService = FakeRoleService();
            sessionService = FakeSessionService();
            roleService.AssignFunctionToRole(Arg.Any<AssignFunctionToRoleCriteria>()).Returns(new CommandResult());
            roleController = GetRoleController(roleService, sessionService);

            // Act
            var actual = roleController.AssignFunctionToRole(Arg.Any<AssignFunctionToRoleCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion AssignFunctionToRole

        #region Helpers

        private ISessionService FakeSessionService()
        {
            ISessionService service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private IRoleService FakeRoleService()
        {
            IRoleService service = Substitute.For<IRoleService>();

            return service;
        }

        private RoleController GetRoleController(IRoleService roleService = null, ISessionService sessionService = null)
        {
            var controller = new RoleController(roleService);
            controller.SessionService = sessionService;

            return controller;
        }

        #endregion Helpers
    }
}