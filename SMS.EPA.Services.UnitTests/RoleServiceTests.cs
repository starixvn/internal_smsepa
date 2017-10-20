namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Repositories;
    using Criteria;
    using Implement;
    using Model;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class RoleServiceTests
    {
        private RoleService _roleService;

        #region GetFunctionByRoleId

        [Test]
        public void GetFunctionByRoleId_AnyArg_ReturnInstanceOfListOfRoleFunctionModel()
        {
            // Arrange
            int roleId = 1;
            ArrangeGetFunctionByRoleId();

            // Act
            var actual = _roleService.GetFunctionByRoleId(roleId);

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<RoleFunctionModel>), actual);
        }

        [Test]
        public void GetFunctionByRoleId_ValidArg_AssignChildFunctionsToParentFunctions()
        {
            // Arrange
            int roleId = 1;
            IEnumerable<RoleFunctionModel> result = GetListOfRoleFunctionModelForTest();
            ArrangeGetFunctionByRoleId(result);

            // Act
            IEnumerable<RoleFunctionModel> functions = _roleService.GetFunctionByRoleId(roleId);

            // Assert
            Assert.AreEqual(1, functions.Count());
            Assert.AreEqual(result.ElementAt(1), functions.First().ChildFunctions.First());
        }

        #endregion GetFunctionByRoleId

        #region Helpers

        private IBaseRepository FakeBaseRepository()
        {
            var repository = Substitute.For<IBaseRepository>();

            return repository;
        }

        private RoleService GetRoleService(IBaseRepository baseRepository = null)
        {
            var service = new RoleService();
            service.Repository = baseRepository;

            return service;
        }

        private void ArrangeGetFunctionByRoleId(IEnumerable<RoleFunctionModel> result = null)
        {
            _roleService = Substitute.For<RoleService>();
            _roleService.When(x => x.GetData<RoleFunctionModel>(Arg.Any<GetFunctionByRoleIdCriteria>())).DoNotCallBase();
            _roleService.GetData<RoleFunctionModel>(Arg.Any<GetFunctionByRoleIdCriteria>()).Returns(result ?? new List<RoleFunctionModel>());
        }

        private IEnumerable<RoleFunctionModel> GetListOfRoleFunctionModelForTest()
        {
            var functions = new List<RoleFunctionModel>
            {
                new RoleFunctionModel
                {
                    FunctionId = 1,
                    FunctionKey = "EPA",
                    FunctionName = "EPA"
                },
                new RoleFunctionModel
                {
                    FunctionId = 2,
                    FunctionKey = "Evaluator",
                    FunctionName = "Evaluator",
                    ParentFunctionId = 1
                }
            };

            return functions;
        }

        #endregion Helpers
    }
}