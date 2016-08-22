namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Repositories;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Implement;

    [TestFixture]
    public class RoleServiceTests
    {
        private RoleService roleService;
        private IBaseRepository baseRepository;

        #region GetFunctionByRoleId

        [Test]
        public void GetFunctionByRoleId_AnyArg_ReturnInstanceOfListOfRoleFunctionModel()
        {
            // Arrange
            int roleId = 1;
            ArrangeGetFunctionByRoleId();

            // Act
            var actual = roleService.GetFunctionByRoleId(roleId);

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<RoleFunctionModel>), actual);
        }

        [Test]
        public void GetFunctionByRoleId_ValidArg_ExpectedToExecuteGetData()
        {
            // Arrange
            int roleId = 1;
            ArrangeGetFunctionByRoleId();

            // Act
            roleService.GetFunctionByRoleId(roleId);

            // Assert
            roleService.Received().GetData<RoleFunctionModel>(Arg.Is<GetFunctionByRoleIdCriteria>(x => x.IsValid()));
        }

        [Test]
        public void GetFunctionByRoleId_ValidArg_AssignChildFunctionsToParentFunctions()
        {
            // Arrange
            int roleId = 1;
            IEnumerable<RoleFunctionModel> result = GetListOfRoleFonctionModelForTest();
            ArrangeGetFunctionByRoleId(result);

            // Act
            IEnumerable<RoleFunctionModel> functions = roleService.GetFunctionByRoleId(roleId);

            // Assert
            Assert.AreEqual(1, functions.Count());
            Assert.AreEqual(result.ElementAt(1), functions.First().ChildFunctions.First());
        }

        #endregion GetFunctionByRoleId

        #region AssignFunctionToRole

        [Test]
        public void AssignFunctionToRole_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeAssignFunctionToRole();

            // Act
            var actual = roleService.AssignFunctionToRole(new AssignFunctionToRoleCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void AssignFunctionToRole_ValidArg_ExpectedToExecuteUpdate()
        {
            // Arrange
            var criteria = new AssignFunctionToRoleCriteria
            {
                RoleId = 1,
                Functions = "<ROOT><Function FunctionId=\"1\" IsRead=\"1\" IsHost=\"0\"></Function></ROOT>",
                ModifiedBy = 1
            };
            ArrangeAssignFunctionToRole();

            // Act
            roleService.AssignFunctionToRole(criteria);

            // Assert
            roleService.Repository.Received().Update(Arg.Is<AssignFunctionToRoleCriteria>(x => x.IsValid()));
        }

        #endregion AssignFunctionToRole

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
            roleService = Substitute.For<RoleService>();
            roleService.When(x => x.GetData<RoleFunctionModel>(Arg.Any<GetFunctionByRoleIdCriteria>())).DoNotCallBase();
            roleService.GetData<RoleFunctionModel>(Arg.Any<GetFunctionByRoleIdCriteria>()).Returns(result ?? new List<RoleFunctionModel>());
        }

        private void ArrangeAssignFunctionToRole(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            roleService = GetRoleService(baseRepository);
            roleService.Repository.Update(Arg.Any<AssignFunctionToRoleCriteria>()).Returns(result ?? new CommandResult());
        }

        private IEnumerable<RoleFunctionModel> GetListOfRoleFonctionModelForTest()
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