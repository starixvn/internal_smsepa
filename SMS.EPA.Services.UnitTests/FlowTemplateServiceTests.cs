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
    using Repositories;

    [TestFixture]
    public class FlowTemplateServiceTests
    {
        private FlowTemplateService _flowTemplateService;
        private IFlowTemplateRepository _flowTemplateRepository;

        #region GetFlowTemplate

        [Test]
        public void GetFlowTemplate_AnyArg_ReturnInstanceOfListOfFlowTemplateModel()
        {
            // Arrange
            _flowTemplateService = Substitute.For<FlowTemplateService>();

            // Act
            var actual = _flowTemplateService.GetFlowTemplate(Arg.Any<GetFlowTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<FlowTemplateModel>), actual);
        }

        [Test]
        public void GetFlowTemplate_ValidArg_ReturnExpectedFlowTemplates()
        {
            // Arrange
            var criteria = new GetFlowTemplateCriteria();
            IEnumerable<FlowTemplateModel> result = GetListOfFlowTemplateForTest();
            ArrangeGetFlowTemplate(result);

            // Act
            var actual = _flowTemplateService.GetFlowTemplate(criteria);

            // Assert
            Assert.AreEqual(result, actual);
        }

        [Test]
        public void GetFlowTemplate_ValidArg_FlowTemplateIncludeTargetType()
        {
            // Arrange
            var criteria = new GetFlowTemplateCriteria();
            IEnumerable<FlowTemplateModel> result = GetListOfFlowTemplateForTest();
            ArrangeGetFlowTemplate(result);

            // Act
            var actual = _flowTemplateService.GetFlowTemplate(criteria);

            // Assert
            Assert.AreEqual(0, actual.Where(x => x.TargetType == null || x.TargetType.TargetTypeId <= 0).Count());
        }

        #endregion GetFlowTemplate

        #region GetFlowTemplateById

        [Test]
        public void GetFlowTemplateById_AnyArg_ReturnInstanceOfFlowTemplateModel()
        {
            // Arrange
            int flowTemplateId = 1;
            ArrangeGetFlowTemplateById();

            // Act
            var actual = _flowTemplateService.GetFlowTemplateById(flowTemplateId);

            // Assert
            Assert.IsInstanceOf(typeof(FlowTemplateModel), actual);
        }

        [Test]
        public void GetFlowTemplateById_ValidArg_ReturnExpectedFlowTemplate()
        {
            // Arrange
            int flowTemplateId = 1;
            FlowTemplateModel result = GetListOfFlowTemplateForTest().First();
            ArrangeGetFlowTemplateById(result);

            // Act
            var actual = _flowTemplateService.GetFlowTemplateById(flowTemplateId);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(result, actual);
            Assert.AreEqual(flowTemplateId, actual.FlowTemplateId);
        }

        [Test]
        public void GetFlowTemplateById_ValidArg_FlowTemplateIncludeTargetType()
        {
            // Arrange
            int flowTemplateId = 1;
            FlowTemplateModel result = GetListOfFlowTemplateForTest().First();
            ArrangeGetFlowTemplateById(result);

            // Act
            var actual = _flowTemplateService.GetFlowTemplateById(flowTemplateId);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(false, actual.TargetType == null || actual.TargetType.TargetTypeId <= 0);
        }

        #endregion GetFlowTemplateById

        #region Helpers

        private IBaseRepository FakeBaseRepository()
        {
            var repository = Substitute.For<IBaseRepository>();

            return repository;
        }

        private IFlowTemplateRepository FakeFlowTemplateRepository()
        {
            var repository = Substitute.For<IFlowTemplateRepository>();

            return repository;
        }

        private FlowTemplateService GetFlowTemplateService(
            IFlowTemplateRepository flowTemplateRepository,
            IBaseRepository baseRepository = null)
        {
            var service = new FlowTemplateService(flowTemplateRepository);
            service.Repository = baseRepository;

            return service;
        }

        private IEnumerable<FlowTemplateModel> GetListOfFlowTemplateForTest()
        {
            var flowTemplates = new List<FlowTemplateModel>
            {
                new FlowTemplateModel
                {
                    FlowTemplateId = 1,
                    Name = "test 1",
                    TargetType = new TargetTypeModel
                    {
                        TargetTypeId = 1,
                        Name = "Monthly"
                    }
                },
                new FlowTemplateModel
                {
                    FlowTemplateId = 2,
                    Name = "test 2",
                    TargetType = new TargetTypeModel
                    {
                        TargetTypeId = 2,
                        Name = "Semi-annual"
                    }
                }
            };

            return flowTemplates;
        }

        private void ArrangeGetFlowTemplate(IEnumerable<FlowTemplateModel> result = null)
        {
            _flowTemplateRepository = FakeFlowTemplateRepository();
            _flowTemplateRepository.GetFlowTemplate(Arg.Any<GetFlowTemplateCriteria>()).Returns(result ?? new List<FlowTemplateModel>());
            _flowTemplateService = GetFlowTemplateService(_flowTemplateRepository);
        }

        private void ArrangeGetFlowTemplateById(FlowTemplateModel result = null)
        {
            _flowTemplateRepository = FakeFlowTemplateRepository();
            _flowTemplateRepository.GetFlowTemplateById(Arg.Any<GetFlowTemplateByIdCriteria>()).Returns(result ?? new FlowTemplateModel());
            _flowTemplateService = GetFlowTemplateService(_flowTemplateRepository);
        }

        #endregion Helpers
    }
}