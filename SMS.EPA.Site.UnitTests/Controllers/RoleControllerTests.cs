namespace SMS.EPA.Site.UnitTests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Model;
    using NSubstitute;
    using NUnit.Framework;
    using Services.Interfaces;
    using Site.Controllers.API;

    [TestFixture]
    public class RoleControllerTests
    {
        private IRoleService _roleService;
        private ICommonService commonService;
        private ISessionService _sessionService;
        private RoleController _roleController;

        #region GetRoleFunctionByRoleId

        [Test]
        public void GetRoleFunctionByRoleId_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _roleController = GetRoleController();

            // Act
            var actual = _roleController.GetType().GetMethod("GetRoleFunctionByRoleId");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetRoleFunctionByRoleId_WhenCalled_ReturnInstanceOfListOfRoleFunctionModel()
        {
            // Arrange
            _roleService = FakeRoleService();
            _roleService.GetFunctionByRoleId(Arg.Any<int>()).Returns(new List<RoleFunctionModel>());
            _roleController = GetRoleController(_roleService);

            // Act
            var actual = _roleController.GetRoleFunctionByRoleId(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<RoleFunctionModel>), actual);
        }

        #endregion GetRoleFunctionByRoleId

        #region AssignFunctionToRole

        [Test]
        public void AssignFunctionToRole_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _roleService = FakeRoleService();
            _sessionService = FakeSessionService();
            commonService = FakeCommonService();
            commonService.ExecuteNonQuery(Arg.Any<AssignFunctionToRoleCriteria>()).Returns(new CommandResult());
            _roleController = GetRoleController(_roleService, _sessionService, commonService);

            // Act
            var actual = _roleController.AssignFunctionToRole(Arg.Any<AssignFunctionToRoleCriteria>());

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

        private ICommonService FakeCommonService()
        {
            var service = Substitute.For<ICommonService>();

            return service;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Reviewed")]
        private RoleController GetRoleController(
            IRoleService roleService = null,
            ISessionService sessionService = null,
            ICommonService commonService = null)
        {
            var controller = new RoleController(roleService, commonService);
            controller.SessionService = sessionService;

            return controller;
        }

        #endregion Helpers
    }
}