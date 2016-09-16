namespace SMS.EPA.Site.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
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
    public class FlowTemplateControllerTests
    {
        private IFlowTemplateService flowTemplateService;
        private ISessionService sessionService;
        private FlowTemplateController flowTemplateController;

        #region GetFlowTemplate

        [Test]
        public void GetFlowTemplate_WhenCalled_ReturnListFlowTemplates()
        {
            // Arrange
            IEnumerable<FlowTemplateModel> result = GetListOfFlowTemplateForTest();
            flowTemplateService = FakeFlowTemplateService();
            flowTemplateService.GetFlowTemplate(Arg.Any<GetFlowTemplateCriteria>()).Returns(result);
            flowTemplateController = GetFlowTemplateController(flowTemplateService);

            // Act
            var actual = flowTemplateController.GetFlowTemplate(Arg.Any<int?>(), Arg.Any<bool?>());

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(result, actual);
        }

        #endregion GetFlowTemplate

        #region GetFlowTemplateById

        [Test]
        public void GetFlowTemplateById_WhenCalled_ReturnExpectedFlowTemplate()
        {
            // Arrange
            int flowTemplateId = 1;
            FlowTemplateModel result = GetListOfFlowTemplateForTest().First();
            flowTemplateService = FakeFlowTemplateService();
            flowTemplateService.GetFlowTemplateById(flowTemplateId).Returns(result);
            flowTemplateController = GetFlowTemplateController(flowTemplateService);

            // Act
            var actual = flowTemplateController.GetFlowTemplateById(flowTemplateId);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(flowTemplateId, actual.FlowTemplateId);
        }

        #endregion GetFlowTemplateById

        #region CreateFlowTemplate

        [Test]
        public void CreateFlowTemplate_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            flowTemplateService = FakeFlowTemplateService();
            sessionService = FakeSessionService();
            flowTemplateService.CreateFlowTemplate(Arg.Any<CreateFlowTemplateCriteria>()).Returns(new CommandResult());
            flowTemplateController = GetFlowTemplateController(flowTemplateService, sessionService);

            // Act
            var actual = flowTemplateController.CreateFlowTemplate(Arg.Any<CreateFlowTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion CreateFlowTemplate

        #region UpdateFlowTemplate

        [Test]
        public void UpdateFlowTemplate_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            flowTemplateService = FakeFlowTemplateService();
            sessionService = FakeSessionService();
            flowTemplateService.UpdateFlowTemplate(Arg.Any<UpdateFlowTemplateCriteria>()).Returns(new CommandResult());
            flowTemplateController = GetFlowTemplateController(flowTemplateService, sessionService);

            // Act
            var actual = flowTemplateController.UpdateFlowTemplate(Arg.Any<UpdateFlowTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateFlowTemplate

        #region UpdateStatusFlowTemplate

        [Test]
        public void UpdateStatusFlowTemplate_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            flowTemplateService = FakeFlowTemplateService();
            sessionService = FakeSessionService();
            flowTemplateService.UpdateStatusFlowTemplate(Arg.Any<UpdateStatusFlowTemplateCriteria>()).Returns(new CommandResult());
            flowTemplateController = GetFlowTemplateController(flowTemplateService, sessionService);

            // Act
            var actual = flowTemplateController.UpdateStatusFlowTemplate(Arg.Any<UpdateStatusFlowTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateStatusFlowTemplate

        #region DeleteFlowTemplate

        [Test]
        public void DeleteFlowTemplate_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            flowTemplateService = FakeFlowTemplateService();
            sessionService = FakeSessionService();
            flowTemplateService.DeleteFlowTemplate(Arg.Any<DeleteFlowTemplateCriteria>()).Returns(new CommandResult());
            flowTemplateController = GetFlowTemplateController(flowTemplateService, sessionService);

            // Act
            var actual = flowTemplateController.DeleteFlowTemplate(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion DeleteFlowTemplate

        #region Helpers

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private IFlowTemplateService FakeFlowTemplateService()
        {
            var service = Substitute.For<IFlowTemplateService>();

            return service;
        }

        private FlowTemplateController GetFlowTemplateController(
            IFlowTemplateService flowTemplateService,
            ISessionService sessionService = null)
        {
            var controller = new FlowTemplateController(flowTemplateService);
            controller.SessionService = sessionService;

            return controller;
        }

        private IEnumerable<FlowTemplateModel> GetListOfFlowTemplateForTest()
        {
            var flowTemplates = new List<FlowTemplateModel>
            {
                new FlowTemplateModel
                {
                    FlowTemplateId = 1,
                    Name = "test 1",
                    IsActive = true,
                    TargetType = new TargetType
                    {
                        TargetTypeId = 1,
                        Name = "Monthly"
                    }
                },
                new FlowTemplateModel
                {
                    FlowTemplateId = 2,
                    Name = "test 2",
                    IsActive = false,
                    TargetType = new TargetType
                    {
                        TargetTypeId = 2,
                        Name = "Semi-annual"
                    }
                }
            };

            return flowTemplates;
        }

        #endregion Helpers
    }
}