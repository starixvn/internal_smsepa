namespace SMS.EPA.Site.UnitTests
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
    public class RoleControllerTests
    {
        private IRoleService _roleService;
        private IBaseService baseService;
        private ISessionService _sessionService;
        private RoleController _roleController;

        #region GetRoleFunctionByRoleId

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
        public void AssignFunctionToRole_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            _roleService = FakeRoleService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<AssignFunctionToRoleCriteria>()).Returns(expected);
            _roleController = GetRoleController(_roleService, _sessionService, baseService);

            // Act
            var actual = _roleController.AssignFunctionToRole(Arg.Any<AssignFunctionToRoleCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion AssignFunctionToRole

        #region Helpers

        private static ISessionService FakeSessionService()
        {
            ISessionService service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private static IRoleService FakeRoleService()
        {
            IRoleService service = Substitute.For<IRoleService>();

            return service;
        }

        private static IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private RoleController GetRoleController(
            IRoleService roleService = null,
            ISessionService sessionService = null,
            IBaseService baseService = null)
        {
            var controller = new RoleController(roleService, baseService) { SessionService = sessionService };

            return controller;
        }

        #endregion Helpers
    }
}