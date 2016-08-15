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
        private IBaseRepository baseRepository;

        [Test]
        public void GetFunctionByRoleId_WhenCalled_ReturnsListOfRoleFunctionModel()
        {
            // Arrange
            var service = Substitute.For<RoleService>();
            var expected = new List<RoleFunctionModel>();
            service.When(x => x.GetData<RoleFunctionModel>(Arg.Any<GetFunctionByRoleIdCriteria>())).DoNotCallBase();
            service.GetData<RoleFunctionModel>(Arg.Any<GetFunctionByRoleIdCriteria>()).Returns(expected);

            // Act
            var actual = service.GetFunctionByRoleId(new GetFunctionByRoleIdCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<RoleFunctionModel>), actual);
        }

        [Test]
        public void GetFunctionByRoleId_WhenCalled_FunctionsIncludeChildFunctions()
        {
            // Arrange
            var service = Substitute.For<RoleService>();
            IEnumerable<RoleFunctionModel> result = GetListOfRoleFonctionModelForTest();
            service.When(x => x.GetData<RoleFunctionModel>(Arg.Any<GetFunctionByRoleIdCriteria>())).DoNotCallBase();
            service.GetData<RoleFunctionModel>(Arg.Any<GetFunctionByRoleIdCriteria>()).Returns(result);

            // Act
            var criteria = new GetFunctionByRoleIdCriteria
            {
                RoleId = 1
            };
            IEnumerable<RoleFunctionModel> functions = service.GetFunctionByRoleId(criteria);

            // Assert
            Assert.AreEqual(1, functions.Count());
            Assert.AreEqual(result.ElementAt(1), functions.First().ChildFunctions.First());
        }

        [Test]
        public void AssignFunctionToRole_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            baseRepository = Substitute.For<IBaseRepository>();
            var service = GetRoleService(baseRepository);
            var expected = new CommandResult();
            service.Repository.Update(Arg.Is<AssignFunctionToRoleCriteria>(criteria => criteria != null)).Returns(expected);

            // Act
            var actual = service.AssignFunctionToRole(new AssignFunctionToRoleCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
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