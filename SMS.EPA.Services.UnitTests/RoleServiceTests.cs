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

        [Test]
        public void GetFunctionByRoleId_AnyArg_ReturnInstanceOfListOfRoleFunctionModel()
        {
            // Arrange
            roleService = Substitute.For<RoleService>();
            roleService.When(x => x.GetData<RoleFunctionModel>(Arg.Any<GetFunctionByRoleIdCriteria>())).DoNotCallBase();
            roleService.GetData<RoleFunctionModel>(Arg.Any<GetFunctionByRoleIdCriteria>()).Returns(new List<RoleFunctionModel>());

            // Act
            var actual = roleService.GetFunctionByRoleId(new GetFunctionByRoleIdCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<RoleFunctionModel>), actual);
        }

        [Test]
        public void GetFunctionByRoleId_ValidArg_ExpectedToExecuteGetData()
        {
            // Arrange
            var criteria = new GetFunctionByRoleIdCriteria
            {
                RoleId = 1
            };
            roleService = Substitute.For<RoleService>();
            roleService.When(x => x.GetData<RoleFunctionModel>(Arg.Any<GetFunctionByRoleIdCriteria>())).DoNotCallBase();
            roleService.GetData<RoleFunctionModel>(Arg.Any<GetFunctionByRoleIdCriteria>()).Returns(new List<RoleFunctionModel>());

            // Act
            roleService.GetFunctionByRoleId(criteria);

            // Assert
            roleService.Received().GetData<RoleFunctionModel>(Arg.Is<GetFunctionByRoleIdCriteria>(x => x.IsValid()));
        }

        [Test]
        public void GetFunctionByRoleId_ValidArg_AssignChildFunctionsToParentFunctions()
        {
            // Arrange
            var criteria = new GetFunctionByRoleIdCriteria
            {
                RoleId = 1
            };
            roleService = Substitute.For<RoleService>();
            IEnumerable<RoleFunctionModel> result = GetListOfRoleFonctionModelForTest();
            roleService.When(x => x.GetData<RoleFunctionModel>(Arg.Any<GetFunctionByRoleIdCriteria>())).DoNotCallBase();
            roleService.GetData<RoleFunctionModel>(Arg.Any<GetFunctionByRoleIdCriteria>()).Returns(result);

            // Act
            IEnumerable<RoleFunctionModel> functions = roleService.GetFunctionByRoleId(criteria);

            // Assert
            Assert.AreEqual(1, functions.Count());
            Assert.AreEqual(result.ElementAt(1), functions.First().ChildFunctions.First());
        }

        [Test]
        public void AssignFunctionToRole_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            baseRepository = Substitute.For<IBaseRepository>();
            roleService = GetRoleService(baseRepository);
            roleService.Repository.Update(Arg.Any<AssignFunctionToRoleCriteria>()).Returns(new CommandResult());

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
            baseRepository = Substitute.For<IBaseRepository>();
            roleService = GetRoleService(baseRepository);
            roleService.Repository.Update(Arg.Any<AssignFunctionToRoleCriteria>()).Returns(new CommandResult());

            // Act
            roleService.AssignFunctionToRole(criteria);

            // Assert
            roleService.Repository.Received().Update(Arg.Is<AssignFunctionToRoleCriteria>(x => x.IsValid()));
        }

        private RoleService GetRoleService(IBaseRepository baseRepository = null)
        {
            var service = new RoleService();
            service.Repository = baseRepository;

            return service;
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
    }
}