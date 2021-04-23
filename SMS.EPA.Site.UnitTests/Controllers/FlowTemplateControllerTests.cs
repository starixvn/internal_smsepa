namespace SMS.EPA.Site.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Model;
    using NSubstitute;
    using NUnit.Framework;
    using Services.Interfaces;
    using Site.Controllers.API;

    [TestFixture]
    public class FlowTemplateControllerTests
    {
        private IFlowTemplateService _flowTemplateService;
        private IBaseService baseService;
        private ISessionService _sessionService;
        private FlowTemplateController _flowTemplateController;

        #region GetFlowTemplate

        [Test]
        public void GetFlowTemplate_WhenCalled_ReturnListFlowTemplates()
        {
            // Arrange
            IEnumerable<FlowTemplateModel> result = GetListOfFlowTemplateForTest();
            _flowTemplateService = FakeFlowTemplateService();
            _flowTemplateService.GetFlowTemplate(Arg.Any<GetFlowTemplateCriteria>()).Returns(result);
            _flowTemplateController = GetFlowTemplateController(_flowTemplateService);

            // Act
            var actual = _flowTemplateController.GetFlowTemplate(Arg.Any<int?>(), Arg.Any<bool?>());

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
            _flowTemplateService = FakeFlowTemplateService();
            _flowTemplateService.GetFlowTemplateById(flowTemplateId).Returns(result);
            _flowTemplateController = GetFlowTemplateController(_flowTemplateService);

            // Act
            var actual = _flowTemplateController.GetFlowTemplateById(flowTemplateId);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(flowTemplateId, actual.FlowTemplateId);
        }

        #endregion GetFlowTemplateById

        #region CreateFlowTemplate

        [Test]
        public void CreateFlowTemplate_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            _flowTemplateService = FakeFlowTemplateService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<CreateFlowTemplateCriteria>()).Returns(expected);
            _flowTemplateController = GetFlowTemplateController(_flowTemplateService, _sessionService, baseService);

            // Act
            var actual = _flowTemplateController.CreateFlowTemplate(Arg.Any<CreateFlowTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion CreateFlowTemplate

        #region UpdateFlowTemplate

        [Test]
        public void UpdateFlowTemplate_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            _flowTemplateService = FakeFlowTemplateService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<UpdateFlowTemplateCriteria>()).Returns(expected);
            _flowTemplateController = GetFlowTemplateController(_flowTemplateService, _sessionService, baseService);

            // Act
            var actual = _flowTemplateController.UpdateFlowTemplate(Arg.Any<UpdateFlowTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion UpdateFlowTemplate

        #region UpdateStatusFlowTemplate

        [Test]
        public void UpdateStatusFlowTemplate_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            _flowTemplateService = FakeFlowTemplateService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<UpdateStatusFlowTemplateCriteria>()).Returns(expected);
            _flowTemplateController = GetFlowTemplateController(_flowTemplateService, _sessionService, baseService);

            // Act
            var actual = _flowTemplateController.UpdateStatusFlowTemplate(Arg.Any<UpdateStatusFlowTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion UpdateStatusFlowTemplate

        #region DeleteFlowTemplate

        [Test]
        public void DeleteFlowTemplate_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            _flowTemplateService = FakeFlowTemplateService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<DeleteFlowTemplateCriteria>()).Returns(expected);
            _flowTemplateController = GetFlowTemplateController(_flowTemplateService, _sessionService, baseService);

            // Act
            var actual = _flowTemplateController.DeleteFlowTemplate(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion DeleteFlowTemplate

        #region Helpers

        private static ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private static IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private static IFlowTemplateService FakeFlowTemplateService()
        {
            var service = Substitute.For<IFlowTemplateService>();

            return service;
        }

        private FlowTemplateController GetFlowTemplateController(
            IFlowTemplateService flowTemplateService,
            ISessionService sessionService = null,
            IBaseService baseService = null)
        {
            var controller = new FlowTemplateController(flowTemplateService, baseService) { SessionService = sessionService };

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
                    IsActive = false,
                    TargetType = new TargetTypeModel
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