namespace SMS.EPA.Site.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using GO.UserControl.Client.Core.Session;
    using Model;
    using NSubstitute;
    using NUnit.Framework;
    using Services.Interfaces;
    using Site.Controllers.API;
    using SMS.EPA.Core.Constants;

    [TestFixture]
    public class FlowTemplateControllerTests
    {
        private IFlowTemplateService flowTemplateService;
        private IBaseService baseService;
        private IUserControlSession userControlSession;

        #region GetFlowTemplate

        [Test]
        public void GetFlowTemplate_WhenCalled_ReturnListFlowTemplates()
        {
            // Arrange
            IEnumerable<FlowTemplateModel> result = GetListOfFlowTemplateForTest();
            var controller = GetFlowTemplateController();
            flowTemplateService.GetFlowTemplate(Arg.Any<GetFlowTemplateCriteria>()).Returns(result);

            // Act
            var actual = controller.GetFlowTemplate(Arg.Any<int?>(), Arg.Any<bool?>());

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
            var controller = GetFlowTemplateController();
            flowTemplateService.GetFlowTemplateById(flowTemplateId).Returns(result);

            // Act
            var actual = controller.GetFlowTemplateById(flowTemplateId);

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
            var controller = GetFlowTemplateController();
            baseService.ExecuteNonQuery(Arg.Any<CreateFlowTemplateCriteria>()).Returns(expected);

            // Act
            var actual = controller.CreateFlowTemplate(Arg.Any<CreateFlowTemplateCriteria>());

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
            var controller = GetFlowTemplateController();
            baseService.ExecuteNonQuery(Arg.Any<UpdateFlowTemplateCriteria>()).Returns(expected);

            // Act
            var actual = controller.UpdateFlowTemplate(Arg.Any<UpdateFlowTemplateCriteria>());

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
            var controller = GetFlowTemplateController();
            baseService.ExecuteNonQuery(Arg.Any<UpdateStatusFlowTemplateCriteria>()).Returns(expected);

            // Act
            var actual = controller.UpdateStatusFlowTemplate(Arg.Any<UpdateStatusFlowTemplateCriteria>());

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
            var controller = GetFlowTemplateController();
            baseService.ExecuteNonQuery(Arg.Any<DeleteFlowTemplateCriteria>()).Returns(expected);

            // Act
            var actual = controller.DeleteFlowTemplate(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion DeleteFlowTemplate

        #region Helpers

        private static IUserControlSession FakeUserControlSession()
        {
            var session = Substitute.For<IUserControlSession>();
            session.Set<int>(Constants.CurrentEmployeeIdKey, 1);

            return session;
        }

        private FlowTemplateController GetFlowTemplateController()
        {
            userControlSession = FakeUserControlSession();
            flowTemplateService = Substitute.For<IFlowTemplateService>();
            baseService = Substitute.For<IBaseService>();
            var controller = new FlowTemplateController(flowTemplateService, baseService)
            {
                UserControlSession = userControlSession
            };

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