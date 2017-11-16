namespace SMS.EPA.Site.UnitTests.Controllers
{
    using System.Collections.Generic;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Fanex.Data.Repository;
    using Model;
    using Model.Enitites;
    using NSubstitute;
    using NUnit.Framework;
    using Services.Interfaces;
    using Site.Controllers.API;

    [TestFixture]
    public class DepartmentFlowControllerTests
    {
        private IDepartmentFlowService _departmentFlowService;
        private DepartmentFlowController _departmentFlowController;
        private IBaseService baseService;

        #region GetDepartmentFlowByFlowTemplateId

        [Test]
        public void GetDepartmentFlowByFlowTemplateId_AnyArg_ReturnExpectedDepartmentFlows()
        {
            // Arrange
            var expectedDepartmentFlows = new List<DepartmentFlowModel>();
            expectedDepartmentFlows.Add(new DepartmentFlowModel());
            ArrangeGetDepartmentFlowByFlowTemplateId(expectedDepartmentFlows);

            // Act
            var actual = _departmentFlowController.GetDepartmentFlowByFlowTemplateId(Arg.Any<int>());

            // Assert
            Assert.AreEqual(expectedDepartmentFlows, actual);
        }

        [Test]
        public void GetTargetEmployeeSetting_AnyArg_ReturnListOfDepartmentFlow()
        {
            // Arrange
            ArrangeGetDepartmentFlowByFlowTemplateId();

            // Act
            var actual = _departmentFlowController.GetDepartmentFlowByFlowTemplateId(Arg.Any<int>());

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
            var actual = _departmentFlowController.GetDepartmentFlowById(Arg.Any<int>(), Arg.Any<int>());

            // Assert
            Assert.AreEqual(expectedFlows, actual);
        }

        [Test]
        public void GetDepartmentFlowById_AnyArg_ReturnListOfFlow()
        {
            // Arrange
            ArrangeGetDepartmentFlowById();

            // Act
            var actual = _departmentFlowController.GetDepartmentFlowById(Arg.Any<int>(), Arg.Any<int>());

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
            var actual = _departmentFlowController.CreateDepartmentFlow(null);

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
            var actual = _departmentFlowController.CreateDepartmentFlow(new CreateDepartmentFlowCriteria());

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
            var actual = _departmentFlowController.CreateDepartmentFlow(null);

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
            var actual = _departmentFlowController.DeleteDepartmentFlow(Arg.Any<int>(), Arg.Any<int>());

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
            var actual = _departmentFlowController.DeleteDepartmentFlow(Arg.Any<int>(), Arg.Any<int>());

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
            var actual = _departmentFlowController.DeleteDepartmentFlow(Arg.Any<int>(), Arg.Any<int>());

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
            var actual = _departmentFlowController.GetDepartmentHasSetting(Arg.Any<int>());

            // Assert
            Assert.AreEqual(expectedDepartments, actual);
        }

        [Test]
        public void GetDepartmentHasSetting_AnyArg_ReturnListOfDepartment()
        {
            // Arrange
            ArrangeGetDepartmentHasSetting();

            // Act
            var actual = _departmentFlowController.GetDepartmentHasSetting(Arg.Any<int>());

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

        private IDepartmentFlowService FakeDepartmentFlowService()
        {
            var service = Substitute.For<IDepartmentFlowService>();

            return service;
        }

        private IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private void ArrangeGetDepartmentFlowByFlowTemplateId(IList<DepartmentFlowModel> departmentFlows = null)
        {
            _departmentFlowService = FakeDepartmentFlowService();
            baseService = FakeBaseService();

            _departmentFlowService.GetDepartmentFlowByFlowTemplateId(Arg.Any<int>()).Returns(departmentFlows ?? new List<DepartmentFlowModel>());
            _departmentFlowController = new DepartmentFlowController(_departmentFlowService, baseService);
        }

        private void ArrangeGetDepartmentFlowById(IList<FlowModel> flows = null)
        {
            _departmentFlowService = FakeDepartmentFlowService();
            baseService = FakeBaseService();

            _departmentFlowService.GetDepartmentFlowById(Arg.Any<GetDepartmentFlowByIdCriteria>()).Returns(flows ?? new List<FlowModel>());
            _departmentFlowController = new DepartmentFlowController(_departmentFlowService, baseService);
        }

        private void ArrangeCreateDepartmentFlow(CommandResult result = null)
        {
            _departmentFlowService = FakeDepartmentFlowService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<ICriteria>()).Returns(result ?? new CommandResult());
            _departmentFlowController = new DepartmentFlowController(_departmentFlowService, baseService);
            _departmentFlowController.SessionService = FakeSessionService();
        }

        private void ArrangeDeleteDepartmentFlow(CommandResult result = null)
        {
            _departmentFlowService = FakeDepartmentFlowService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<ICriteria>()).Returns(result ?? new CommandResult());
            _departmentFlowController = new DepartmentFlowController(_departmentFlowService, baseService);
            _departmentFlowController.SessionService = FakeSessionService();
        }

        private void ArrangeGetDepartmentHasSetting(IList<Department> departments = null)
        {
            _departmentFlowService = FakeDepartmentFlowService();
            baseService = FakeBaseService();

            baseService.GetData<Department>(Arg.Any<GetDepartmentSectionHasSettingCriteria>()).Returns(departments ?? new List<Department>());
            _departmentFlowController = new DepartmentFlowController(_departmentFlowService, baseService);
        }
    }
}