namespace SMS.EPA.Services.UnitTests
{
    using Core.Model;
    using Core.Repositories;
    using Criteria;
    using Implement;
    using Model;
    using NSubstitute;
    using NUnit.Framework;
    using Repositories;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class FlowTemplateServiceTests
    {
        private FlowTemplateService _flowTemplateService;
        private IFlowTemplateRepository _flowTemplateRepository;
        private IBaseRepository _baseRepository;

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

        #region CreateFlowTemplate

        [Test]
        public void CreateFlowTemplate_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeCreateFlowTemplate();

            // Act
            var actual = _flowTemplateService.CreateFlowTemplate(new CreateFlowTemplateCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void CreateFlowTemplate_ValidArg_ExpectedToExecuteInsert()
        {
            // Arrange
            var criteria = new CreateFlowTemplateCriteria
            {
                Name = "test",
                TargetTypeId = 1,
                CreatedBy = 1
            };
            ArrangeCreateFlowTemplate();

            // Act
            _flowTemplateService.CreateFlowTemplate(criteria);

            // Assert
            _flowTemplateService.Repository.Received().Insert(Arg.Is<CreateFlowTemplateCriteria>(x => x.IsValid()));
        }

        #endregion CreateFlowTemplate

        #region UpdateFlowTemplate

        [Test]
        public void UpdateFlowTemplate_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeUpdateFlowTemplate();

            // Act
            var actual = _flowTemplateService.UpdateFlowTemplate(new UpdateFlowTemplateCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateFlowTemplate_ValidArg_ExpectedToExecuteUpdate()
        {
            // Arrange
            var criteria = new UpdateFlowTemplateCriteria
            {
                FlowTemplateId = 1,
                Name = "test",
                TargetTypeId = 1,
                ModifiedBy = 1
            };
            ArrangeUpdateFlowTemplate();

            // Act
            _flowTemplateService.UpdateFlowTemplate(criteria);

            // Assert
            _flowTemplateService.Repository.Received().Update(Arg.Is<UpdateFlowTemplateCriteria>(x => x.IsValid()));
        }

        #endregion UpdateFlowTemplate

        #region UpdateStatusFlowTemplate

        [Test]
        public void UpdateStatusFlowTemplate_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeUpdateStatusFlowTemplate();

            // Act
            var actual = _flowTemplateService.UpdateStatusFlowTemplate(new UpdateStatusFlowTemplateCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateStatusFlowTemplate_ValidArg_ExpectedToExecuteUpdate()
        {
            // Arrange
            var criteria = new UpdateStatusFlowTemplateCriteria
            {
                FlowTemplateId = 1,
                ModifiedBy = 1
            };
            ArrangeUpdateStatusFlowTemplate();

            // Act
            _flowTemplateService.UpdateStatusFlowTemplate(criteria);

            // Assert
            _flowTemplateService.Repository.Received().Update(Arg.Is<UpdateStatusFlowTemplateCriteria>(x => x.IsValid()));
        }

        #endregion UpdateStatusFlowTemplate

        #region DeleteFlowTemplate

        [Test]
        public void DeleteFlowTemplate_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeDeleteFlowTemplate();

            // Act
            var actual = _flowTemplateService.DeleteFlowTemplate(new DeleteFlowTemplateCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void DeleteFlowTemplate_ValidArg_ExpectedToExecuteDelete()
        {
            // Arrange
            var criteria = new DeleteFlowTemplateCriteria
            {
                FlowTemplateId = 1,
                ModifiedBy = 1
            };
            ArrangeDeleteFlowTemplate();

            // Act
            _flowTemplateService.DeleteFlowTemplate(criteria);

            // Assert
            _flowTemplateService.Repository.Received().Delete(Arg.Is<DeleteFlowTemplateCriteria>(x => x.IsValid()));
        }

        #endregion DeleteFlowTemplate

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

        private void ArrangeCreateFlowTemplate(CommandResult result = null)
        {
            _baseRepository = FakeBaseRepository();
            _flowTemplateService = GetFlowTemplateService(null, _baseRepository);
            _flowTemplateService.Repository.Insert(Arg.Any<CreateFlowTemplateCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeUpdateFlowTemplate(CommandResult result = null)
        {
            _baseRepository = FakeBaseRepository();
            _flowTemplateService = GetFlowTemplateService(null, _baseRepository);
            _flowTemplateService.Repository.Update(Arg.Any<UpdateFlowTemplateCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeUpdateStatusFlowTemplate(CommandResult result = null)
        {
            _baseRepository = FakeBaseRepository();
            _flowTemplateService = GetFlowTemplateService(null, _baseRepository);
            _flowTemplateService.Repository.Update(Arg.Any<UpdateStatusFlowTemplateCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeDeleteFlowTemplate(CommandResult result = null)
        {
            _baseRepository = FakeBaseRepository();
            _flowTemplateService = GetFlowTemplateService(null, _baseRepository);
            _flowTemplateService.Repository.Delete(Arg.Any<DeleteFlowTemplateCriteria>()).Returns(result ?? new CommandResult());
        }

        #endregion Helpers
    }
}