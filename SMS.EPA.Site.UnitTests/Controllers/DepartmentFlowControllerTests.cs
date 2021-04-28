namespace SMS.EPA.Site.UnitTests
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
        public void GetDepartmentFlowByFlowTemplateId_WhenCalled_ReturnExpectedDepartmentFlows()
        {
            // Arrange
            var expected = new List<DepartmentFlowModel> { new DepartmentFlowModel() };
            ArrangeGetDepartmentFlowByFlowTemplateId(expected);

            // Act
            var actual = _departmentFlowController.GetDepartmentFlowByFlowTemplateId(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<DepartmentFlowModel>), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion GetDepartmentFlowByFlowTemplateId

        #region GetDepartmentFlowById

        [Test]
        public void GetDepartmentFlowById_WhenCalled_ReturnExpectedFlows()
        {
            // Arrange
            var expectedFlows = new List<FlowModel> { new FlowModel() };
            ArrangeGetDepartmentFlowById(expectedFlows);

            // Act
            var actual = _departmentFlowController.GetDepartmentFlowById(Arg.Any<int>(), Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<FlowModel>), actual);
            Assert.AreEqual(expectedFlows, actual);
        }

        #endregion GetDepartmentFlowById

        #region CreateDepartmentFlow

        [Test]
        public void CreateDepartmentFlow_CriteriaIsNull_ReturnDefaultCommandResult()
        {
            // Arrange
            var expectedCommandResult = new CommandResult();
            ArrangeCreateDepartmentFlow();

            // Act
            var actual = _departmentFlowController.CreateDepartmentFlow(null);

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expectedCommandResult.Code, actual.Code);
        }

        [Test]
        public void CreateDepartmentFlow_CriteriaIsNotNull_ReturnExpectedCommandResult()
        {
            // Arrange
            var expectedCommandResult = new CommandResult
            {
                Code = 1
            };
            ArrangeCreateDepartmentFlow(expectedCommandResult);

            // Act
            var actual = _departmentFlowController.CreateDepartmentFlow(new CreateDepartmentFlowCriteria());

            // Assert
            Assert.AreEqual(expectedCommandResult, actual);
        }

        #endregion CreateDepartmentFlow

        #region DeleteDepartmentFlow

        [Test]
        public void DeleteDepartmentFlow_CriteriaIsNull_ReturnDefaultCommandResult()
        {
            // Arrange
            var expectedCommandResult = new CommandResult();
            ArrangeDeleteDepartmentFlow();

            // Act
            var actual = _departmentFlowController.DeleteDepartmentFlow(Arg.Any<int>(), Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expectedCommandResult.Code, actual.Code);
        }

        [Test]
        public void DeleteDepartmentFlow_CriteriaIsNotNull_ReturnExpectedCommandResult()
        {
            // Arrange
            var expectedCommandResult = new CommandResult
            {
                Code = 1
            };
            ArrangeDeleteDepartmentFlow(expectedCommandResult);

            // Act
            var actual = _departmentFlowController.DeleteDepartmentFlow(Arg.Any<int>(), Arg.Any<int>());

            // Assert
            Assert.AreEqual(expectedCommandResult, actual);
        }

        #endregion DeleteDepartmentFlow

        #region GetDepartmentHasSetting

        [Test]
        public void GetDepartmentHasSetting_WhenCalled_ReturnExpectedDepartments()
        {
            // Arrange
            var expectedDepartments = new List<Department> { new Department() };
            ArrangeGetDepartmentHasSetting(expectedDepartments);

            // Act
            var actual = _departmentFlowController.GetDepartmentHasSetting(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(IList<Department>), actual);
            Assert.AreEqual(expectedDepartments, actual);
        }

        #endregion GetDepartmentHasSetting

        private static ISessionService FakeSessionService(byte? permission = null)
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;
            service.GetPermission(Arg.Any<string>()).Returns(permission ?? 0);

            return service;
        }

        private static IDepartmentFlowService FakeDepartmentFlowService()
        {
            var service = Substitute.For<IDepartmentFlowService>();

            return service;
        }

        private static IBaseService FakeBaseService()
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
            _departmentFlowController = new DepartmentFlowController(_departmentFlowService, baseService)
            {
                SessionService = FakeSessionService()
            };
        }

        private void ArrangeDeleteDepartmentFlow(CommandResult result = null)
        {
            _departmentFlowService = FakeDepartmentFlowService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<ICriteria>()).Returns(result ?? new CommandResult());
            _departmentFlowController = new DepartmentFlowController(_departmentFlowService, baseService)
            {
                SessionService = FakeSessionService()
            };
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