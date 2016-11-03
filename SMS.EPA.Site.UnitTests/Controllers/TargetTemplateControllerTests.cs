namespace SMS.EPA.Site.UnitTests
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
    public class TargetTemplateControllerTests
    {
        private ITargetTemplateService targetTemplateService;
        private IScoreRatingTypeService scoreRatingTypeService;
        private ISessionService sessionService;
        private TargetTemplateController targetTemplateController;

        #region GetAllTargetTemplate

        [Test]
        public void GetAllTargetTemplate_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = targetTemplateController.GetType().GetMethod("GetAllTargetTemplate");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetAllTargetTemplate_WhenCalled_ReturnInstanceOfListOfTargetTemplateModel()
        {
            // Arrange
            targetTemplateService = FakeTargetTemplateService();
            targetTemplateService.GetAllTargetTemplate(Arg.Any<GetAllTargetTemplateCriteria>()).Returns(new List<TargetTemplate>());
            targetTemplateController = GetTargetTemplateController(targetTemplateService);

            // Act
            var actual = targetTemplateController.GetAllTargetTemplate(Arg.Any<int?>(), Arg.Any<bool?>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<TargetTemplate>), actual);
        }

        #endregion GetAllTargetTemplate

        #region GetTargetTemplateById

        [Test]
        public void GetTargetTemplateById_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = targetTemplateController.GetType().GetMethod("GetTargetTemplateById");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetTargetTemplateById_WhenCalled_ReturnInstanceOfTargetTemplateModel()
        {
            // Arrange
            targetTemplateService = FakeTargetTemplateService();
            targetTemplateService.GetTargetTemplateById(Arg.Any<GetTargetTemplateByIdCriteria>()).Returns(new TargetTemplate());
            targetTemplateController = GetTargetTemplateController(targetTemplateService);

            // Act
            var actual = targetTemplateController.GetTargetTemplateById(Arg.Is<int>(x => x > 0));

            // Assert
            Assert.IsInstanceOf(typeof(TargetTemplate), actual);
        }

        #endregion GetTargetTemplateById

        #region CreateTargetTemplate

        [Test]
        public void CreateTargetTemplate_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = targetTemplateController.GetType().GetMethod("CreateTargetTemplate");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void CreateTargetTemplate_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            targetTemplateService = FakeTargetTemplateService();
            sessionService = FakeSessionService();
            targetTemplateService.CreateTargetTemplate(Arg.Any<CreateTargetTemplateCriteria>()).Returns(new CommandResult());
            targetTemplateController = GetTargetTemplateController(targetTemplateService, null, sessionService);

            // Act
            var actual = targetTemplateController.CreateTargetTemplate(Arg.Any<CreateTargetTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion CreateTargetTemplate

        #region UpdateStatusTargetTemplate

        [Test]
        public void UpdateStatusTargetTemplate_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = targetTemplateController.GetType().GetMethod("UpdateStatusTargetTemplate");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void UpdateStatusTargetTemplate_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            targetTemplateService = FakeTargetTemplateService();
            sessionService = FakeSessionService();
            targetTemplateService.UpdateStatusTargetTemplate(Arg.Any<UpdateStatusTargetTemplateCriteria>()).Returns(new CommandResult());
            targetTemplateController = GetTargetTemplateController(targetTemplateService, null, sessionService);

            // Act
            var actual = targetTemplateController.UpdateStatusTargetTemplate(Arg.Any<UpdateStatusTargetTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateStatusTargetTemplate

        #region UpdateTargetTemplateBasicInfo

        [Test]
        public void UpdateTargetTemplateBasicInfo_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = targetTemplateController.GetType().GetMethod("UpdateTargetTemplateBasicInfo");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void UpdateTargetTemplateBasicInfo_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            targetTemplateService = FakeTargetTemplateService();
            sessionService = FakeSessionService();
            targetTemplateService.UpdateTargetTemplateBasicInfo(Arg.Any<UpdateTargetTemplateBasicInfoCriteria>()).Returns(new CommandResult());
            targetTemplateController = GetTargetTemplateController(targetTemplateService, null, sessionService);

            // Act
            var actual = targetTemplateController.UpdateTargetTemplateBasicInfo(Arg.Any<UpdateTargetTemplateBasicInfoCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateTargetTemplateBasicInfo

        #region UpdateTargetTemplate

        [Test]
        public void UpdateTargetTemplate_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = targetTemplateController.GetType().GetMethod("UpdateTargetTemplate");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void UpdateTargetTemplate_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            targetTemplateService = FakeTargetTemplateService();
            sessionService = FakeSessionService();
            targetTemplateService.UpdateTargetTemplate(Arg.Any<UpdateTargetTemplateCriteria>()).Returns(new CommandResult());
            targetTemplateController = GetTargetTemplateController(targetTemplateService, null, sessionService);

            // Act
            var actual = targetTemplateController.UpdateTargetTemplate(Arg.Any<UpdateTargetTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateTargetTemplate

        #region DeleteTargetTemplate

        [Test]
        public void DeleteTargetTemplate_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = targetTemplateController.GetType().GetMethod("DeleteTargetTemplate");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void DeleteTargetTemplate_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            targetTemplateService = FakeTargetTemplateService();
            sessionService = FakeSessionService();
            targetTemplateService.DeleteTargetTemplate(Arg.Any<DeleteTargetTemplateCriteria>()).Returns(new CommandResult());
            targetTemplateController = GetTargetTemplateController(targetTemplateService, null, sessionService);

            // Act
            var actual = targetTemplateController.DeleteTargetTemplate(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion DeleteTargetTemplate

        #region AddKPIGroupWeight

        [Test]
        public void AddKPIGroupWeight_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = targetTemplateController.GetType().GetMethod("AddKPIGroupWeight");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void AddKPIGroupWeight_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            targetTemplateService = FakeTargetTemplateService();
            sessionService = FakeSessionService();
            targetTemplateService.AddKPIGroupWeight(Arg.Any<SetKPIGroupOfTargetTemplateCriteria>()).Returns(new CommandResult());
            targetTemplateController = GetTargetTemplateController(targetTemplateService, null, sessionService);

            // Act
            var actual = targetTemplateController.AddKPIGroupWeight(Arg.Any<SetKPIGroupOfTargetTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion AddKPIGroupWeight

        #region GetKPIGroupOfTargetTemplate

        [Test]
        public void GetKPIGroupOfTargetTemplate_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = targetTemplateController.GetType().GetMethod("GetKPIGroupOfTargetTemplate");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetKPIGroupOfTargetTemplate_WhenCalled_ReturnInstanceOfListOfKPIGroupModel()
        {
            // Arrange
            targetTemplateService = FakeTargetTemplateService();
            sessionService = FakeSessionService();
            targetTemplateService.GetKPIGroupOfTargetTemplate(Arg.Any<GetKPIGroupOfTargetTemplateCriteria>()).Returns(new List<KPIGroupModel>());
            targetTemplateController = GetTargetTemplateController(targetTemplateService, null, sessionService);

            // Act
            var actual = targetTemplateController.GetKPIGroupOfTargetTemplate(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<KPIGroupModel>), actual);
        }

        #endregion GetKPIGroupOfTargetTemplate

        #region GetKPIOfTargetTemplate

        [Test]
        public void GetKPIOfTargetTemplate_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = targetTemplateController.GetType().GetMethod("GetKPIOfTargetTemplate");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetKPIOfTargetTemplate_WhenCalled_ReturnInstanceOfListOfTargetTemplateKPIGroupModel()
        {
            // Arrange
            targetTemplateService = FakeTargetTemplateService();
            sessionService = FakeSessionService();
            targetTemplateService.GetKPIOfTargetTemplate(Arg.Any<GetKPIOfTargetTemplateCriteria>(), Arg.Any<bool>()).Returns(new List<TargetTemplateKPIGroupModel>());
            targetTemplateController = GetTargetTemplateController(targetTemplateService, null, sessionService);

            // Act
            var actual = targetTemplateController.GetKPIOfTargetTemplate(Arg.Any<int>(), false);

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<TargetTemplateKPIGroupModel>), actual);
        }

        #endregion GetKPIOfTargetTemplate

        #region DeleteKPIGroupOfTargetTemplate

        [Test]
        public void DeleteKPIGroupOfTargetTemplate_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = targetTemplateController.GetType().GetMethod("DeleteKPIGroupOfTargetTemplate");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void DeleteKPIGroupOfTargetTemplate_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            targetTemplateService = FakeTargetTemplateService();
            sessionService = FakeSessionService();
            targetTemplateService.DeleteKPIGroup(Arg.Any<DeleteKPIGroupOfTargetTemplateCriteria>()).Returns(new CommandResult());
            targetTemplateController = GetTargetTemplateController(targetTemplateService, null, sessionService);

            // Act
            var actual = targetTemplateController.DeleteKPIGroupOfTargetTemplate(Arg.Any<int>(), Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion DeleteKPIGroupOfTargetTemplate

        #region GetScoreRatingType

        [Test]
        public void GetScoreRatingType_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = targetTemplateController.GetType().GetMethod("GetScoreRatingType");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetScoreRatingType_WhenCalled_ReturnInstanceOfListOfScoreRatingTypeModel()
        {
            // Arrange
            scoreRatingTypeService = FakeScoreRatingTypeService();
            sessionService = FakeSessionService();
            scoreRatingTypeService.GetScoreRatingType(Arg.Any<GetScoreRatingTypeCriteria>()).Returns(new List<ScoreRatingTypeModel>());
            targetTemplateController = GetTargetTemplateController(null, scoreRatingTypeService, sessionService);

            // Act
            var actual = targetTemplateController.GetScoreRatingType(Arg.Any<int?>(), Arg.Any<bool>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<ScoreRatingTypeModel>), actual);
        }

        #endregion GetScoreRatingType

        #region GetRatingType

        [Test]
        public void GetRatingType_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = targetTemplateController.GetType().GetMethod("GetRatingType");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetRatingType_WhenCalled_ReturnInstanceOfListOfRatingType()
        {
            // Arrange
            targetTemplateService = FakeTargetTemplateService();
            sessionService = FakeSessionService();
            targetTemplateService.GetRatingType(Arg.Any<GetRatingTypeCriteria>()).Returns(new List<RatingType>());
            targetTemplateController = GetTargetTemplateController(targetTemplateService, null, sessionService);

            // Act
            var actual = targetTemplateController.GetRatingType();

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<RatingType>), actual);
        }

        #endregion GetRatingType

        #region Helpers

        private ISessionService FakeSessionService()
        {
            ISessionService service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private ITargetTemplateService FakeTargetTemplateService()
        {
            ITargetTemplateService service = Substitute.For<ITargetTemplateService>();

            return service;
        }

        private IScoreRatingTypeService FakeScoreRatingTypeService()
        {
            IScoreRatingTypeService service = Substitute.For<IScoreRatingTypeService>();

            return service;
        }

        private TargetTemplateController GetTargetTemplateController(
            ITargetTemplateService targetTemplateService = null,
            IScoreRatingTypeService scoreRatingTypeService = null,
            ISessionService sessionService = null)
        {
            var controller = new TargetTemplateController(targetTemplateService, scoreRatingTypeService);
            controller.SessionService = sessionService;

            return controller;
        }

        #endregion Helpers
    }
}