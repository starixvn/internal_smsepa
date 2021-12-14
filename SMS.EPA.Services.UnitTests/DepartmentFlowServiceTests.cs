namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using Criteria;
    using Model;
    using NSubstitute;
    using NUnit.Framework;
    using Repositories;
    using SMS.EPA.Core.Services;

    [TestFixture]
    public class DepartmentFlowServiceTests
    {
        private DepartmentFlowService departmentFlowService;
        private IBaseService baseService;
        private IFlowSettingRepository flowSettingRepository;

        #region GetById

        [Test]
        public void GetById_AnyArg_ReturnInstanceOfListOfDepartmentFlowModel()
        {
            // Arrange
            ArrangeGetDepartmentFlowById();

            // Act
            var actual = departmentFlowService.GetById(null);

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<FlowModel>), actual);
        }

        [Test]
        public void GetById_VaidArg_ReturnExpectedFlows()
        {
            // Arrange
            var citeria = new GetDepartmentFlowByIdCriteria
            {
                DepartmentId = 1,
                FlowTemplateId = 1
            };
            IEnumerable<FlowModel> expectedFlows = GetListOfFlowForTest();
            ArrangeGetDepartmentFlowById(expectedFlows);

            // Act
            var actual = departmentFlowService.GetById(citeria);

            // Assert
            Assert.AreEqual(expectedFlows, actual);
        }

        [Test]
        public void GetById_InVaidArg_ReturnEmptyFlows()
        {
            // Arrange
            IEnumerable<FlowModel> expectedFlows = GetListOfFlowForTest();
            ArrangeGetDepartmentFlowById(expectedFlows);

            // Act
            var actual = departmentFlowService.GetById(null);

            // Assert
            Assert.IsEmpty(actual);
        }

        #endregion GetById

        #region Helpers

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

        private void ArrangeGetDepartmentFlowById(IEnumerable<FlowModel> departmentFlows = null)
        {
            baseService = Substitute.For<IBaseService>();
            flowSettingRepository = Substitute.For<IFlowSettingRepository>();

            flowSettingRepository.GetFlowSettingByFlowTemplateId(Arg.Any<GetDepartmentFlowByIdCriteria>())
                .Returns(departmentFlows ?? new List<FlowModel>());
            departmentFlowService = new DepartmentFlowService(baseService, flowSettingRepository);
        }

        #endregion Helpers
    }
}