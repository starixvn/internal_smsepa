namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using Core.Model;
    using Core.Repositories;
    using Criteria;
    using Fanex.Data.Repository;
    using Implement;
    using Model;
    using NSubstitute;
    using NUnit.Framework;
    using Repositories;

    [TestFixture]
    public class DepartmentFlowServiceTests
    {
        private DepartmentFlowService departmentFlowService;
        private IFlowSettingRepository flowSettingRepository;

        #region GetDepartmentFlowByFlowTemplateId

        [Test]
        public void GetDepartmentFlowByFlowTemplateId_AnyArg_ReturnInstanceOfListOfDepartmentFlowModel()
        {
            // Arrange
            ArrangeGetDepartmentFlowByFlowTemplateId();

            // Act
            var actual = departmentFlowService.GetDepartmentFlowByFlowTemplateId(0);

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<DepartmentFlowModel>), actual);
        }

        [Test]
        public void GetDepartmentFlowByFlowTemplateId_VaidArg_ReturnExpectedDepartmentFlows()
        {
            // Arrange
            int flowTemplateId = 1;
            var expectedDepartmentFlows = GetListOfDepartmentFlowForTest();
            ArrangeGetDepartmentFlowByFlowTemplateId(expectedDepartmentFlows);

            // Act
            var actual = departmentFlowService.GetDepartmentFlowByFlowTemplateId(flowTemplateId);

            // Assert
            Assert.AreEqual(expectedDepartmentFlows, actual);
        }

        [Test]
        public void GetDepartmentFlowByFlowTemplateId_InVaidArg_ReturnEmptyDepartmentFlows()
        {
            // Arrange
            int flowTemplateId = 0;
            var expectedDepartmentFlows = GetListOfDepartmentFlowForTest();
            ArrangeGetDepartmentFlowByFlowTemplateId(expectedDepartmentFlows);

            // Act
            var actual = departmentFlowService.GetDepartmentFlowByFlowTemplateId(flowTemplateId);

            // Assert
            Assert.IsEmpty(actual);
        }

        #endregion GetDepartmentFlowByFlowTemplateId

        #region GetDepartmentFlowById

        [Test]
        public void GetDepartmentFlowById_AnyArg_ReturnInstanceOfListOfDepartmentFlowModel()
        {
            // Arrange
            ArrangeGetDepartmentFlowById();

            // Act
            var actual = departmentFlowService.GetDepartmentFlowById(null);

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<FlowModel>), actual);
        }

        [Test]
        public void GetDepartmentFlowById_VaidArg_ReturnExpectedFlows()
        {
            // Arrange
            var citeria = new GetDepartmentFlowByIdCriteria()
            {
                DepartmentId = 1,
                FlowTemplateId = 1
            };
            IEnumerable<FlowModel> expectedFlows = GetListOfFlowForTest();
            ArrangeGetDepartmentFlowById(expectedFlows);

            // Act
            var actual = departmentFlowService.GetDepartmentFlowById(citeria);

            // Assert
            Assert.AreEqual(expectedFlows, actual);
        }

        [Test]
        public void GetDepartmentFlowById_InVaidArg_ReturnEmptyFlows()
        {
            // Arrange
            IEnumerable<FlowModel> expectedFlows = GetListOfFlowForTest();
            ArrangeGetDepartmentFlowById(expectedFlows);

            // Act
            var actual = departmentFlowService.GetDepartmentFlowById(null);

            // Assert
            Assert.IsEmpty(actual);
        }

        #endregion GetDepartmentFlowById

        #region Helpers

        private IBaseRepository FakeBaseRepository()
        {
            var repository = Substitute.For<IBaseRepository>();

            return repository;
        }

        private IFlowSettingRepository FakeFlowSettingRepository()
        {
            var repository = Substitute.For<IFlowSettingRepository>();

            return repository;
        }

        private IEnumerable<DepartmentFlowModel> GetListOfDepartmentFlowForTest()
        {
            var flowTemplates = new List<DepartmentFlowModel>
            {
                new DepartmentFlowModel
                {
                    DepartmentId = 1,
                    Code = "HR",
                    Name = "HR",
                    HumanResources = new List<ProcessFlowModel>
                    {
                        new ProcessFlowModel { EmployeeId = 1, FullName = "a" }
                    }
                },
                new DepartmentFlowModel
                {
                    DepartmentId = 2,
                    Code = "Management",
                    Name = "Management"
                }
            };

            return flowTemplates;
        }

        private IEnumerable<FlowModel> GetListOfFlowForTest()
        {
            var flows = new List<FlowModel>
            {
                new FlowModel
                {
                    ProcessId = 1
                }
            };

            return flows;
        }

        private void ArrangeGetDepartmentFlowByFlowTemplateId(IEnumerable<DepartmentFlowModel> departmentFlows = null)
        {
            flowSettingRepository = FakeFlowSettingRepository();

            flowSettingRepository.GetDepartmentFlowByFlowTemplateId(Arg.Any<GetDepartmentFlowByFlowTemplateIdCriteria>())
                .Returns(departmentFlows ?? new List<DepartmentFlowModel>());
            departmentFlowService = new DepartmentFlowService(flowSettingRepository);
        }

        private void ArrangeGetDepartmentFlowById(IEnumerable<FlowModel> departmentFlows = null)
        {
            flowSettingRepository = FakeFlowSettingRepository();

            flowSettingRepository.GetFlowSettingByFlowTemplateId(Arg.Any<GetDepartmentFlowByIdCriteria>())
                .Returns(departmentFlows ?? new List<FlowModel>());
            departmentFlowService = new DepartmentFlowService(flowSettingRepository);
        }

        #endregion Helpers
    }
}