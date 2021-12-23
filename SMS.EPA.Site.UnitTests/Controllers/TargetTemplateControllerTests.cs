namespace SMS.EPA.Site.UnitTests
{
    using System.Collections.Generic;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using HRM.UserControl.Client.Core.Session;
    using Model;
    using Model.Enitites;
    using NSubstitute;
    using NUnit.Framework;
    using Services.Interfaces;
    using Site.Controllers.API;
    using SMS.EPA.Core.Constants;

    [TestFixture]
    public class TargetTemplateControllerTests
    {
        private ITargetTemplateService targetTemplateService;
        private IBaseService baseService;
        private IUserControlSession userControlSession;

        [Test]
        public void GetAllTargetTemplate_WhenCalled_ReturnInstanceOfListOfTargetTemplateModel()
        {
            // Arrange
            var controller = GetTargetTemplateController();
            baseService.GetData<TargetTemplate>(Arg.Any<GetAllTargetTemplateCriteria>()).Returns(new List<TargetTemplate>());

            // Act
            var actual = controller.GetAllTargetTemplate(Arg.Any<int?>(), Arg.Any<bool?>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<TargetTemplate>), actual);
        }

        [Test]
        public void GetTargetTemplateById_WhenCalled_ReturnInstanceOfTargetTemplateModel()
        {
            // Arrange
            var controller = GetTargetTemplateController();
            baseService.GetDataEntity<TargetTemplate>(Arg.Any<GetTargetTemplateByIdCriteria>()).Returns(new TargetTemplate());

            // Act
            var actual = controller.GetTargetTemplateById(Arg.Is<int>(x => x > 0));

            // Assert
            Assert.IsInstanceOf(typeof(TargetTemplate), actual);
        }

        [Test]
        public void CreateTargetTemplate_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetTargetTemplateController();
            baseService.ExecuteNonQuery(Arg.Any<CreateTargetTemplateCriteria>()).Returns(expected);

            // Act
            var actual = controller.CreateTargetTemplate(new CreateTargetTemplateCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UpdateStatusTargetTemplate_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetTargetTemplateController();
            baseService.ExecuteNonQuery(Arg.Any<UpdateStatusTargetTemplateCriteria>()).Returns(expected);

            // Act
            var actual = controller.UpdateStatusTargetTemplate(Arg.Any<UpdateStatusTargetTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UpdateTargetTemplateBasicInfo_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetTargetTemplateController();
            baseService.ExecuteNonQuery(Arg.Any<UpdateTargetTemplateBasicInfoCriteria>()).Returns(expected);

            // Act
            var actual = controller.UpdateTargetTemplateBasicInfo(Arg.Any<UpdateTargetTemplateBasicInfoCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UpdateTargetTemplate_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetTargetTemplateController();
            baseService.ExecuteNonQuery(Arg.Any<UpdateTargetTemplateCriteria>()).Returns(expected);

            // Act
            var actual = controller.UpdateTargetTemplate(Arg.Any<UpdateTargetTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DeleteTargetTemplate_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetTargetTemplateController();
            baseService.ExecuteNonQuery(Arg.Any<DeleteTargetTemplateCriteria>()).Returns(expected);

            // Act
            var actual = controller.DeleteTargetTemplate(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddKPIGroupWeight_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetTargetTemplateController();
            baseService.ExecuteNonQuery(Arg.Any<SetKPIGroupOfTargetTemplateCriteria>()).Returns(expected);

            // Act
            var actual = controller.AddKPIGroupWeight(new SetKPIGroupOfTargetTemplateCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void GetKPIGroupOfTargetTemplate_WhenCalled_ReturnInstanceOfListOfKPIGroupModel()
        {
            // Arrange
            var controller = GetTargetTemplateController();
            targetTemplateService.GetKPIGroupOfTargetTemplate(Arg.Any<GetKPIGroupOfTargetTemplateCriteria>()).Returns(new List<KPIGroupModel>());

            // Act
            var actual = controller.GetKPIGroupOfTargetTemplate(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<KPIGroupModel>), actual);
        }

        [Test]
        public void GetKPIOfTargetTemplate_WhenCalled_ReturnInstanceOfListOfTargetTemplateKPIGroupModel()
        {
            // Arrange
            var controller = GetTargetTemplateController();
            targetTemplateService.GetKPIOfTargetTemplate(Arg.Any<GetKPIOfTargetTemplateCriteria>(), Arg.Any<bool>()).Returns(new List<TargetTemplateKPIGroupModel>());

            // Act
            var actual = controller.GetKPIOfTargetTemplate(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<TargetTemplateKPIGroupModel>), actual);
        }

        [Test]
        public void DeleteKPIGroupOfTargetTemplate_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetTargetTemplateController();
            baseService.ExecuteNonQuery(Arg.Any<DeleteKPIGroupOfTargetTemplateCriteria>()).Returns(expected);

            // Act
            var actual = controller.DeleteKPIGroupOfTargetTemplate(Arg.Any<int>(), Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetScoreRatingType_WhenCalled_ReturnInstanceOfListOfScoreRatingTypeModel()
        {
            // Arrange
            var controller = GetTargetTemplateController();
            baseService.GetData<ScoreRatingTypeModel>(Arg.Any<GetScoreRatingTypeCriteria>()).Returns(new List<ScoreRatingTypeModel>());

            // Act
            var actual = controller.GetScoreRatingType(Arg.Any<int?>(), Arg.Any<bool>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<ScoreRatingTypeModel>), actual);
        }

        [Test]
        public void GetRatingType_WhenCalled_ReturnInstanceOfListOfRatingType()
        {
            // Arrange
            var controller = GetTargetTemplateController();
            baseService.GetData<RatingType>(Arg.Any<GetRatingTypeCriteria>()).Returns(new List<RatingType>());

            // Act
            var actual = controller.GetRatingType();

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<RatingType>), actual);
        }

        #region Helpers

        private static IUserControlSession FakeUserControlSession()
        {
            var session = Substitute.For<IUserControlSession>();
            session.Set<int>(Constants.CurrentEmployeeIdKey, 1);

            return session;
        }

        private TargetTemplateController GetTargetTemplateController()
        {
            userControlSession = FakeUserControlSession();
            targetTemplateService = Substitute.For<ITargetTemplateService>();
            baseService = Substitute.For<IBaseService>();
            var controller = new TargetTemplateController(targetTemplateService, baseService)
            {
                UserControlSession = userControlSession
            };

            return controller;
        }

        #endregion Helpers
    }
}