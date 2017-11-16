namespace SMS.EPA.Site.UnitTests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Model;
    using Model.Enitites;
    using NSubstitute;
    using NUnit.Framework;
    using Services.Interfaces;
    using Site.Controllers.API;

    [TestFixture]
    public class TargetTemplateControllerTests
    {
        private ITargetTemplateService _targetTemplateService;
        private IBaseService baseService;
        private ISessionService _sessionService;
        private TargetTemplateController _targetTemplateController;

        [Test]
        public void GetAllTargetTemplate_WhenCalled_ReturnInstanceOfListOfTargetTemplateModel()
        {
            // Arrange
            baseService = FakeBaseService();
            baseService.GetData<TargetTemplate>(Arg.Any<GetAllTargetTemplateCriteria>()).Returns(new List<TargetTemplate>());
            _targetTemplateController = GetTargetTemplateController(null, null, baseService);

            // Act
            var actual = _targetTemplateController.GetAllTargetTemplate(Arg.Any<int?>(), Arg.Any<bool?>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<TargetTemplate>), actual);
        }

        [Test]
        public void GetTargetTemplateById_WhenCalled_ReturnInstanceOfTargetTemplateModel()
        {
            // Arrange
            baseService = FakeBaseService();
            baseService.GetDataEntity<TargetTemplate>(Arg.Any<GetTargetTemplateByIdCriteria>()).Returns(new TargetTemplate());
            _targetTemplateController = GetTargetTemplateController(null, null, baseService);

            // Act
            var actual = _targetTemplateController.GetTargetTemplateById(Arg.Is<int>(x => x > 0));

            // Assert
            Assert.IsInstanceOf(typeof(TargetTemplate), actual);
        }

        [Test]
        public void CreateTargetTemplate_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _targetTemplateService = FakeTargetTemplateService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<CreateTargetTemplateCriteria>()).Returns(new CommandResult());
            _targetTemplateController = GetTargetTemplateController(_targetTemplateService, _sessionService, baseService);

            // Act
            var actual = _targetTemplateController.CreateTargetTemplate(Arg.Any<CreateTargetTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateStatusTargetTemplate_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _targetTemplateService = FakeTargetTemplateService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<UpdateStatusTargetTemplateCriteria>()).Returns(new CommandResult());
            _targetTemplateController = GetTargetTemplateController(_targetTemplateService, _sessionService, baseService);

            // Act
            var actual = _targetTemplateController.UpdateStatusTargetTemplate(Arg.Any<UpdateStatusTargetTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateTargetTemplateBasicInfo_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _targetTemplateService = FakeTargetTemplateService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<UpdateTargetTemplateBasicInfoCriteria>()).Returns(new CommandResult());
            _targetTemplateController = GetTargetTemplateController(_targetTemplateService, _sessionService, baseService);

            // Act
            var actual = _targetTemplateController.UpdateTargetTemplateBasicInfo(Arg.Any<UpdateTargetTemplateBasicInfoCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateTargetTemplate_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _targetTemplateService = FakeTargetTemplateService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<UpdateTargetTemplateCriteria>()).Returns(new CommandResult());
            _targetTemplateController = GetTargetTemplateController(_targetTemplateService, _sessionService, baseService);

            // Act
            var actual = _targetTemplateController.UpdateTargetTemplate(Arg.Any<UpdateTargetTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void DeleteTargetTemplate_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _targetTemplateService = FakeTargetTemplateService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<DeleteTargetTemplateCriteria>()).Returns(new CommandResult());
            _targetTemplateController = GetTargetTemplateController(_targetTemplateService, _sessionService, baseService);

            // Act
            var actual = _targetTemplateController.DeleteTargetTemplate(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void AddKPIGroupWeight_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _targetTemplateService = FakeTargetTemplateService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<SetKPIGroupOfTargetTemplateCriteria>()).Returns(new CommandResult());
            _targetTemplateController = GetTargetTemplateController(_targetTemplateService, _sessionService, baseService);

            // Act
            var actual = _targetTemplateController.AddKPIGroupWeight(Arg.Any<SetKPIGroupOfTargetTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void GetKPIGroupOfTargetTemplate_WhenCalled_ReturnInstanceOfListOfKPIGroupModel()
        {
            // Arrange
            _targetTemplateService = FakeTargetTemplateService();
            _sessionService = FakeSessionService();
            _targetTemplateService.GetKPIGroupOfTargetTemplate(Arg.Any<GetKPIGroupOfTargetTemplateCriteria>()).Returns(new List<KPIGroupModel>());
            _targetTemplateController = GetTargetTemplateController(_targetTemplateService, _sessionService);

            // Act
            var actual = _targetTemplateController.GetKPIGroupOfTargetTemplate(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<KPIGroupModel>), actual);
        }

        [Test]
        public void GetKPIOfTargetTemplate_WhenCalled_ReturnInstanceOfListOfTargetTemplateKPIGroupModel()
        {
            // Arrange
            _targetTemplateService = FakeTargetTemplateService();
            _sessionService = FakeSessionService();
            _targetTemplateService.GetKPIOfTargetTemplate(Arg.Any<GetKPIOfTargetTemplateCriteria>(), Arg.Any<bool>()).Returns(new List<TargetTemplateKPIGroupModel>());
            _targetTemplateController = GetTargetTemplateController(_targetTemplateService, _sessionService);

            // Act
            var actual = _targetTemplateController.GetKPIOfTargetTemplate(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<TargetTemplateKPIGroupModel>), actual);
        }

        [Test]
        public void DeleteKPIGroupOfTargetTemplate_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _targetTemplateService = FakeTargetTemplateService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<DeleteKPIGroupOfTargetTemplateCriteria>()).Returns(new CommandResult());
            _targetTemplateController = GetTargetTemplateController(_targetTemplateService, _sessionService, baseService);

            // Act
            var actual = _targetTemplateController.DeleteKPIGroupOfTargetTemplate(Arg.Any<int>(), Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void GetScoreRatingType_WhenCalled_ReturnInstanceOfListOfScoreRatingTypeModel()
        {
            // Arrange
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.GetData<ScoreRatingTypeModel>(Arg.Any<GetScoreRatingTypeCriteria>()).Returns(new List<ScoreRatingTypeModel>());
            _targetTemplateController = GetTargetTemplateController(null, _sessionService, baseService);

            // Act
            var actual = _targetTemplateController.GetScoreRatingType(Arg.Any<int?>(), Arg.Any<bool>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<ScoreRatingTypeModel>), actual);
        }

        [Test]
        public void GetRatingType_WhenCalled_ReturnInstanceOfListOfRatingType()
        {
            // Arrange
            baseService = FakeBaseService();
            baseService.GetData<RatingType>(Arg.Any<GetRatingTypeCriteria>()).Returns(new List<RatingType>());
            _targetTemplateController = GetTargetTemplateController(null, null, baseService);

            // Act
            var actual = _targetTemplateController.GetRatingType();

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<RatingType>), actual);
        }

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

        private IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Reviewed")]
        private TargetTemplateController GetTargetTemplateController(
            ITargetTemplateService targetTemplateService = null,
            ISessionService sessionService = null,
            IBaseService baseService = null)
        {
            var controller = new TargetTemplateController(targetTemplateService, baseService);
            controller.SessionService = sessionService;

            return controller;
        }

        #endregion Helpers
    }
}