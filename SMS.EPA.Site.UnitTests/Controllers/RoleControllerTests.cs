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
        public void GetRoleFunctionByRoleId_ValidActionName_ReturnAsExpected()
        {
            // Arrange
            roleController = GetRoleController();

            // Act
            var actual = roleController.GetType().GetMethod("GetRoleFunctionByRoleId");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetRoleFunctionByRoleId_WhenCalles_ReturnListOfRoleFunctionModel()
        {
            // Arrange
            roleService = FakeRoleService();
            sessionService = FakeSessionService();
            var expected = new List<RoleFunctionModel>();
            roleService.GetFunctionByRoleId(Arg.Is<GetFunctionByRoleIdCriteria>(criteria => criteria != null)).Returns(expected);
            roleController = GetRoleController(roleService, sessionService);

            // Act
            var actual = roleController.GetRoleFunctionByRoleId(Arg.Is<int>(x => x > 0));

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<RoleFunctionModel>), actual);
        }

        #endregion GetRoleFunctionByRoleId

        #region AssignFunctionToRole

        [Test]
        public void AssignFunctionToRole_ValidActionName_ReturnAsExpected()
        {
            // Arrange
            roleController = GetRoleController();

            // Act
            var actual = roleController.GetType().GetMethod("AssignFunctionToRole");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void AssignFunctionToRole_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            roleService = FakeRoleService();
            sessionService = FakeSessionService();
            var expected = new CommandResult();
            roleService.AssignFunctionToRole(Arg.Is<AssignFunctionToRoleCriteria>(criteria => criteria != null)).Returns(expected);
            roleController = GetRoleController(roleService, sessionService);

            // Act
            var actual = roleController.AssignFunctionToRole(new AssignFunctionToRoleCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion AssignFunctionToRole

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
    }
}