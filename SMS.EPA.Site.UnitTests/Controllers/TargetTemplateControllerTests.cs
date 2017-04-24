namespace SMS.EPA.Site.UnitTests
{
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Model;
    using Model.Enitites;
    using NSubstitute;
    using NUnit.Framework;
    using Services.Interfaces;
    using Site.Controllers.API;
    using System.Collections.Generic;

    [TestFixture]
    public class TargetTemplateControllerTests
    {
        private ITargetTemplateService _targetTemplateService;
        private IScoreRatingTypeService _scoreRatingTypeService;
        private ISessionService _sessionService;
        private TargetTemplateController _targetTemplateController;

        #region GetAllTargetTemplate

        [Test]
        public void GetAllTargetTemplate_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = _targetTemplateController.GetType().GetMethod("GetAllTargetTemplate");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetAllTargetTemplate_WhenCalled_ReturnInstanceOfListOfTargetTemplateModel()
        {
            // Arrange
            _targetTemplateService = FakeTargetTemplateService();
            _targetTemplateService.GetAllTargetTemplate(Arg.Any<GetAllTargetTemplateCriteria>()).Returns(new List<TargetTemplate>());
            _targetTemplateController = GetTargetTemplateController(_targetTemplateService);

            // Act
            var actual = _targetTemplateController.GetAllTargetTemplate(Arg.Any<int?>(), Arg.Any<bool?>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<TargetTemplate>), actual);
        }

        #endregion GetAllTargetTemplate

        #region GetTargetTemplateById

        [Test]
        public void GetTargetTemplateById_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = _targetTemplateController.GetType().GetMethod("GetTargetTemplateById");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetTargetTemplateById_WhenCalled_ReturnInstanceOfTargetTemplateModel()
        {
            // Arrange
            _targetTemplateService = FakeTargetTemplateService();
            _targetTemplateService.GetTargetTemplateById(Arg.Any<GetTargetTemplateByIdCriteria>()).Returns(new TargetTemplate());
            _targetTemplateController = GetTargetTemplateController(_targetTemplateService);

            // Act
            var actual = _targetTemplateController.GetTargetTemplateById(Arg.Is<int>(x => x > 0));

            // Assert
            Assert.IsInstanceOf(typeof(TargetTemplate), actual);
        }

        #endregion GetTargetTemplateById

        #region CreateTargetTemplate

        [Test]
        public void CreateTargetTemplate_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = _targetTemplateController.GetType().GetMethod("CreateTargetTemplate");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void CreateTargetTemplate_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _targetTemplateService = FakeTargetTemplateService();
            _sessionService = FakeSessionService();
            _targetTemplateService.CreateTargetTemplate(Arg.Any<CreateTargetTemplateCriteria>()).Returns(new CommandResult());
            _targetTemplateController = GetTargetTemplateController(_targetTemplateService, null, _sessionService);

            // Act
            var actual = _targetTemplateController.CreateTargetTemplate(Arg.Any<CreateTargetTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion CreateTargetTemplate

        #region UpdateStatusTargetTemplate

        [Test]
        public void UpdateStatusTargetTemplate_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = _targetTemplateController.GetType().GetMethod("UpdateStatusTargetTemplate");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void UpdateStatusTargetTemplate_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _targetTemplateService = FakeTargetTemplateService();
            _sessionService = FakeSessionService();
            _targetTemplateService.UpdateStatusTargetTemplate(Arg.Any<UpdateStatusTargetTemplateCriteria>()).Returns(new CommandResult());
            _targetTemplateController = GetTargetTemplateController(_targetTemplateService, null, _sessionService);

            // Act
            var actual = _targetTemplateController.UpdateStatusTargetTemplate(Arg.Any<UpdateStatusTargetTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateStatusTargetTemplate

        #region UpdateTargetTemplateBasicInfo

        [Test]
        public void UpdateTargetTemplateBasicInfo_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = _targetTemplateController.GetType().GetMethod("UpdateTargetTemplateBasicInfo");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void UpdateTargetTemplateBasicInfo_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _targetTemplateService = FakeTargetTemplateService();
            _sessionService = FakeSessionService();
            _targetTemplateService.UpdateTargetTemplateBasicInfo(Arg.Any<UpdateTargetTemplateBasicInfoCriteria>()).Returns(new CommandResult());
            _targetTemplateController = GetTargetTemplateController(_targetTemplateService, null, _sessionService);

            // Act
            var actual = _targetTemplateController.UpdateTargetTemplateBasicInfo(Arg.Any<UpdateTargetTemplateBasicInfoCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateTargetTemplateBasicInfo

        #region UpdateTargetTemplate

        [Test]
        public void UpdateTargetTemplate_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = _targetTemplateController.GetType().GetMethod("UpdateTargetTemplate");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void UpdateTargetTemplate_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _targetTemplateService = FakeTargetTemplateService();
            _sessionService = FakeSessionService();
            _targetTemplateService.UpdateTargetTemplate(Arg.Any<UpdateTargetTemplateCriteria>()).Returns(new CommandResult());
            _targetTemplateController = GetTargetTemplateController(_targetTemplateService, null, _sessionService);

            // Act
            var actual = _targetTemplateController.UpdateTargetTemplate(Arg.Any<UpdateTargetTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateTargetTemplate

        #region DeleteTargetTemplate

        [Test]
        public void DeleteTargetTemplate_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = _targetTemplateController.GetType().GetMethod("DeleteTargetTemplate");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void DeleteTargetTemplate_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _targetTemplateService = FakeTargetTemplateService();
            _sessionService = FakeSessionService();
            _targetTemplateService.DeleteTargetTemplate(Arg.Any<DeleteTargetTemplateCriteria>()).Returns(new CommandResult());
            _targetTemplateController = GetTargetTemplateController(_targetTemplateService, null, _sessionService);

            // Act
            var actual = _targetTemplateController.DeleteTargetTemplate(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion DeleteTargetTemplate

        #region AddKPIGroupWeight

        [Test]
        public void AddKPIGroupWeight_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = _targetTemplateController.GetType().GetMethod("AddKPIGroupWeight");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void AddKPIGroupWeight_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _targetTemplateService = FakeTargetTemplateService();
            _sessionService = FakeSessionService();
            _targetTemplateService.AddKPIGroupWeight(Arg.Any<SetKPIGroupOfTargetTemplateCriteria>()).Returns(new CommandResult());
            _targetTemplateController = GetTargetTemplateController(_targetTemplateService, null, _sessionService);

            // Act
            var actual = _targetTemplateController.AddKPIGroupWeight(Arg.Any<SetKPIGroupOfTargetTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion AddKPIGroupWeight

        #region GetKPIGroupOfTargetTemplate

        [Test]
        public void GetKPIGroupOfTargetTemplate_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = _targetTemplateController.GetType().GetMethod("GetKPIGroupOfTargetTemplate");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetKPIGroupOfTargetTemplate_WhenCalled_ReturnInstanceOfListOfKPIGroupModel()
        {
            // Arrange
            _targetTemplateService = FakeTargetTemplateService();
            _sessionService = FakeSessionService();
            _targetTemplateService.GetKPIGroupOfTargetTemplate(Arg.Any<GetKPIGroupOfTargetTemplateCriteria>()).Returns(new List<KPIGroupModel>());
            _targetTemplateController = GetTargetTemplateController(_targetTemplateService, null, _sessionService);

            // Act
            var actual = _targetTemplateController.GetKPIGroupOfTargetTemplate(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<KPIGroupModel>), actual);
        }

        #endregion GetKPIGroupOfTargetTemplate

        #region GetKPIOfTargetTemplate

        [Test]
        public void GetKPIOfTargetTemplate_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = _targetTemplateController.GetType().GetMethod("GetKPIOfTargetTemplate");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetKPIOfTargetTemplate_WhenCalled_ReturnInstanceOfListOfTargetTemplateKPIGroupModel()
        {
            // Arrange
            _targetTemplateService = FakeTargetTemplateService();
            _sessionService = FakeSessionService();
            _targetTemplateService.GetKPIOfTargetTemplate(Arg.Any<GetKPIOfTargetTemplateCriteria>(), Arg.Any<bool>()).Returns(new List<TargetTemplateKPIGroupModel>());
            _targetTemplateController = GetTargetTemplateController(_targetTemplateService, null, _sessionService);

            // Act
            var actual = _targetTemplateController.GetKPIOfTargetTemplate(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<TargetTemplateKPIGroupModel>), actual);
        }

        #endregion GetKPIOfTargetTemplate

        #region DeleteKPIGroupOfTargetTemplate

        [Test]
        public void DeleteKPIGroupOfTargetTemplate_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = _targetTemplateController.GetType().GetMethod("DeleteKPIGroupOfTargetTemplate");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void DeleteKPIGroupOfTargetTemplate_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _targetTemplateService = FakeTargetTemplateService();
            _sessionService = FakeSessionService();
            _targetTemplateService.DeleteKPIGroup(Arg.Any<DeleteKPIGroupOfTargetTemplateCriteria>()).Returns(new CommandResult());
            _targetTemplateController = GetTargetTemplateController(_targetTemplateService, null, _sessionService);

            // Act
            var actual = _targetTemplateController.DeleteKPIGroupOfTargetTemplate(Arg.Any<int>(), Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion DeleteKPIGroupOfTargetTemplate

        #region GetScoreRatingType

        [Test]
        public void GetScoreRatingType_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = _targetTemplateController.GetType().GetMethod("GetScoreRatingType");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetScoreRatingType_WhenCalled_ReturnInstanceOfListOfScoreRatingTypeModel()
        {
            // Arrange
            _scoreRatingTypeService = FakeScoreRatingTypeService();
            _sessionService = FakeSessionService();
            _scoreRatingTypeService.GetScoreRatingType(Arg.Any<GetScoreRatingTypeCriteria>()).Returns(new List<ScoreRatingTypeModel>());
            _targetTemplateController = GetTargetTemplateController(null, _scoreRatingTypeService, _sessionService);

            // Act
            var actual = _targetTemplateController.GetScoreRatingType(Arg.Any<int?>(), Arg.Any<bool>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<ScoreRatingTypeModel>), actual);
        }

        #endregion GetScoreRatingType

        #region GetRatingType

        [Test]
        public void GetRatingType_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _targetTemplateController = GetTargetTemplateController();

            // Act
            var actual = _targetTemplateController.GetType().GetMethod("GetRatingType");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetRatingType_WhenCalled_ReturnInstanceOfListOfRatingType()
        {
            // Arrange
            _targetTemplateService = FakeTargetTemplateService();
            _sessionService = FakeSessionService();
            _targetTemplateService.GetRatingType(Arg.Any<GetRatingTypeCriteria>()).Returns(new List<RatingType>());
            _targetTemplateController = GetTargetTemplateController(_targetTemplateService, null, _sessionService);

            // Act
            var actual = _targetTemplateController.GetRatingType();

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Reviewed")]
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