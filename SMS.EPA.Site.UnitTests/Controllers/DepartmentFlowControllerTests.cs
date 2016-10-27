namespace SMS.EPA.Site.UnitTests.Controllers
{
    using System.Collections.Generic;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Services.Interfaces;
    using SMS.EPA.Site.Controllers.API;

    [TestFixture]
    public class DepartmentFlowControllerTests
    {
        private IDepartmentFlowService departmentFlowService;
        private DepartmentFlowController departmentFlowController;

        #region GetDepartmentFlowByFlowTemplateId

        [Test]
        public void GetDepartmentFlowByFlowTemplateId_AnyArg_ReturnExpectedDepartmentFlows()
        {
            // Arrange
            var expectedDepartmentFlows = new List<DepartmentFlowModel>();
            expectedDepartmentFlows.Add(new DepartmentFlowModel());
            ArrangeGetDepartmentFlowByFlowTemplateId(expectedDepartmentFlows);

            // Act
            var actual = departmentFlowController.GetDepartmentFlowByFlowTemplateId(Arg.Any<int>());

            // Assert
            Assert.AreEqual(expectedDepartmentFlows, actual);
        }

        [Test]
        public void GetTargetEmployeeSetting_AnyArg_ReturnListOfDepartmentFlow()
        {
            // Arrange
            ArrangeGetDepartmentFlowByFlowTemplateId();

            // Act
            var actual = departmentFlowController.GetDepartmentFlowByFlowTemplateId(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<DepartmentFlowModel>), actual);
        }

        #endregion GetDepartmentFlowByFlowTemplateId

        #region GetDepartmentFlowById

        [Test]
        public void GetDepartmentFlowById_AnyArg_ReturnExpectedFlows()
        {
            // Arrange
            var expectedFlows = new List<FlowModel>();
            expectedFlows.Add(new FlowModel());
            ArrangeGetDepartmentFlowById(expectedFlows);

            // Act
            var actual = departmentFlowController.GetDepartmentFlowById(Arg.Any<int>(), Arg.Any<int>());

            // Assert
            Assert.AreEqual(expectedFlows, actual);
        }

        [Test]
        public void GetDepartmentFlowById_AnyArg_ReturnListOfFlow()
        {
            // Arrange
            ArrangeGetDepartmentFlowById();

            // Act
            var actual = departmentFlowController.GetDepartmentFlowById(Arg.Any<int>(), Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<FlowModel>), actual);
        }

        #endregion GetDepartmentFlowById

        #region CreateDepartmentFlow

        [Test]
        public void CreateDepartmentFlow_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeCreateDepartmentFlow();

            // Act
            var actual = departmentFlowController.CreateDepartmentFlow(null);

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void CreateDepartmentFlow_CriteriaIsNotNull_ReturnExpectedCommandResult()
        {
            // Arrange
            var expectedCommandResult = new CommandResult()
            {
                Code = 1
            };
            ArrangeCreateDepartmentFlow(expectedCommandResult);

            // Act
            var actual = departmentFlowController.CreateDepartmentFlow(new CreateDepartmentFlowCriteria());

            // Assert
            Assert.AreEqual(expectedCommandResult, actual);
        }

        [Test]
        public void CreateDepartmentFlow_CriteriaIsNull_ReturnDefaultCommandResult()
        {
            // Arrange
            var expectedCommandResult = new CommandResult();
            ArrangeCreateDepartmentFlow();

            // Act
            var actual = departmentFlowController.CreateDepartmentFlow(null);

            // Assert
            Assert.AreEqual(expectedCommandResult.Code, actual.Code);
        }

        #endregion CreateDepartmentFlow

        #region DeleteDepartmentFlow

        [Test]
        public void DeleteDepartmentFlow_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeDeleteDepartmentFlow();

            // Act
            var actual = departmentFlowController.DeleteDepartmentFlow(Arg.Any<int>(), Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void DeleteDepartmentFlow_CriteriaIsNotNull_ReturnExpectedCommandResult()
        {
            // Arrange
            var expectedCommandResult = new CommandResult()
            {
                Code = 1
            };
            ArrangeDeleteDepartmentFlow(expectedCommandResult);

            // Act
            var actual = departmentFlowController.DeleteDepartmentFlow(Arg.Any<int>(), Arg.Any<int>());

            // Assert
            Assert.AreEqual(expectedCommandResult, actual);
        }

        [Test]
        public void DeleteDepartmentFlow_CriteriaIsNull_ReturnDefaultCommandResult()
        {
            // Arrange
            var expectedCommandResult = new CommandResult();
            ArrangeDeleteDepartmentFlow();

            // Act
            var actual = departmentFlowController.DeleteDepartmentFlow(Arg.Any<int>(), Arg.Any<int>());

            // Assert
            Assert.AreEqual(expectedCommandResult.Code, actual.Code);
        }

        #endregion DeleteDepartmentFlow

        #region GetDepartmentHasSetting

        [Test]
        public void GetDepartmentHasSetting_AnyArg_ReturnExpectedDepartments()
        {
            // Arrange
            var expectedDepartments = new List<Department>();
            expectedDepartments.Add(new Department());
            ArrangeGetDepartmentHasSetting(expectedDepartments);

            // Act
            var actual = departmentFlowController.GetDepartmentHasSetting(Arg.Any<int>());

            // Assert
            Assert.AreEqual(expectedDepartments, actual);
        }

        [Test]
        public void GetDepartmentHasSetting_AnyArg_ReturnListOfDepartment()
        {
            // Arrange
            ArrangeGetDepartmentHasSetting();

            // Act
            var actual = departmentFlowController.GetDepartmentHasSetting(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(IList<Department>), actual);
        }

        #endregion GetDepartmentHasSetting

        private ISessionService FakeSessionService(int? permission = null)
        {
            ISessionService service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;
            service.GetPermission(Arg.Any<string>()).Returns(permission.HasValue ? permission.Value : 0);

            return service;
        }

        private void ArrangeGetDepartmentFlowByFlowTemplateId(IList<DepartmentFlowModel> departmentFlows = null)
        {
            departmentFlowService = Substitute.For<IDepartmentFlowService>();

            departmentFlowService.GetDepartmentFlowByFlowTemplateId(Arg.Any<int>()).Returns(departmentFlows ?? new List<DepartmentFlowModel>());
            departmentFlowController = new DepartmentFlowController(departmentFlowService);
        }

        private void ArrangeGetDepartmentFlowById(IList<FlowModel> flows = null)
        {
            departmentFlowService = Substitute.For<IDepartmentFlowService>();

            departmentFlowService.GetDepartmentFlowById(Arg.Any<GetDepartmentFlowByIdCriteria>()).Returns(flows ?? new List<FlowModel>());
            departmentFlowController = new DepartmentFlowController(departmentFlowService);
        }

        private void ArrangeCreateDepartmentFlow(CommandResult result = null)
        {
            departmentFlowService = Substitute.For<IDepartmentFlowService>();
            departmentFlowService.CreateDepartmentFlow(Arg.Any<CreateDepartmentFlowCriteria>()).Returns(result ?? new CommandResult());
            departmentFlowController = new DepartmentFlowController(departmentFlowService);
            departmentFlowController.SessionService = FakeSessionService();
        }

        private void ArrangeDeleteDepartmentFlow(CommandResult result = null)
        {
            departmentFlowService = Substitute.For<IDepartmentFlowService>();
            departmentFlowService.DeleteDepartmentFlow(Arg.Any<DeleteDepartmentFlowCriteria>()).Returns(result ?? new CommandResult());
            departmentFlowController = new DepartmentFlowController(departmentFlowService);
            departmentFlowController.SessionService = FakeSessionService();
        }

        private void ArrangeGetDepartmentHasSetting(IList<Department> departments = null)
        {
            departmentFlowService = Substitute.For<IDepartmentFlowService>();

            departmentFlowService.GetDepartmentHasSettingByFlowTemplateId(Arg.Any<int>()).Returns(departments ?? new List<Department>());
            departmentFlowController = new DepartmentFlowController(departmentFlowService);
        }
    }
}