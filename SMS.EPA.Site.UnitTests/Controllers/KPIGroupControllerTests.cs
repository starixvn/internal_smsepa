namespace SMS.EPA.Site.UnitTests
{
    using System.Collections.Generic;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Interfaces;
    using SMS.EPA.Site.Controllers.API;

    [TestFixture]
    public class KPIGroupControllerTests
    {
        private IKPIGroupService kpiGroupService;
        private ISessionService sessionService;
        private KPIGroupController kpiGroupController;

        #region GetKPIGroup

        [Test]
        public void GetKPIGroup_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            kpiGroupController = GetKPIGroupController();

            // Act
            var actual = kpiGroupController.GetType().GetMethod("GetKPIGroup");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetKPIGroup_WhenCalled_ReturnInstanceOfListOfKPIGroupModel()
        {
            // Arrange
            kpiGroupService = FakeKPIGroupService();
            kpiGroupService.GetKPIGroup(Arg.Any<GetKPIGroupCriteria>()).Returns(new List<KPIGroupModel>());
            kpiGroupController = GetKPIGroupController(kpiGroupService);

            // Act
            var actual = kpiGroupController.GetKPIGroup(Arg.Any<bool?>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<KPIGroupModel>), actual);
        }

        #endregion GetKPIGroup

        #region GetKPIGroupById

        [Test]
        public void GetKPIGroupById_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            kpiGroupController = GetKPIGroupController();

            // Act
            var actual = kpiGroupController.GetType().GetMethod("GetKPIGroupById");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetKPIGroupById_WhenCalled_ReturnInstanceOfKPIGroupModel()
        {
            // Arrange
            kpiGroupService = FakeKPIGroupService();
            kpiGroupService.GetKPIGroupById(Arg.Any<int>()).Returns(new KPIGroupModel());
            kpiGroupController = GetKPIGroupController(kpiGroupService);

            // Act
            var actual = kpiGroupController.GetKPIGroupById(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(KPIGroupModel), actual);
        }

        #endregion GetKPIGroupById

        #region CreateKPIGroup

        [Test]
        public void CreateKPIGroup_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            kpiGroupController = GetKPIGroupController();

            // Act
            var actual = kpiGroupController.GetType().GetMethod("CreateKPIGroup");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void CreateKPIGroup_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            kpiGroupService = FakeKPIGroupService();
            sessionService = FakeSessionService();
            kpiGroupService.CreateKPIGroup(Arg.Any<CreateKPIGroupCriteria>()).Returns(new CommandResult());
            kpiGroupController = GetKPIGroupController(kpiGroupService, sessionService);

            // Act
            var actual = kpiGroupController.CreateKPIGroup(Arg.Any<CreateKPIGroupCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion CreateKPIGroup

        #region UpdateKPIGroup

        [Test]
        public void UpdateKPIGroup_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            kpiGroupController = GetKPIGroupController();

            // Act
            var actual = kpiGroupController.GetType().GetMethod("UpdateKPIGroup");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void UpdateKPIGroup_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            kpiGroupService = FakeKPIGroupService();
            sessionService = FakeSessionService();
            kpiGroupService.UpdateKPIGroup(Arg.Any<UpdateKPIGroupCriteria>()).Returns(new CommandResult());
            kpiGroupController = GetKPIGroupController(kpiGroupService, sessionService);

            // Act
            var actual = kpiGroupController.UpdateKPIGroup(Arg.Any<UpdateKPIGroupCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateKPIGroup

        #region UpdateStatusKPIGroup

        [Test]
        public void UpdateStatusKPIGroup_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            kpiGroupController = GetKPIGroupController();

            // Act
            var actual = kpiGroupController.GetType().GetMethod("UpdateStatusKPIGroup");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void UpdateStatusKPIGroup_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            kpiGroupService = FakeKPIGroupService();
            sessionService = FakeSessionService();
            kpiGroupService.UpdateStatusKPIGroup(Arg.Any<UpdateStatusKPIGroupCriteria>()).Returns(new CommandResult());
            kpiGroupController = GetKPIGroupController(kpiGroupService, sessionService);

            // Act
            var actual = kpiGroupController.UpdateStatusKPIGroup(Arg.Any<UpdateStatusKPIGroupCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateStatusKPIGroup

        #region DeleteKPIGroup

        [Test]
        public void DeleteKPIGroup_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            kpiGroupController = GetKPIGroupController();

            // Act
            var actual = kpiGroupController.GetType().GetMethod("DeleteKPIGroup");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void DeleteKPIGroup_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            kpiGroupService = FakeKPIGroupService();
            sessionService = FakeSessionService();
            kpiGroupService.DeleteKPIGroup(Arg.Any<DeleteKPIGroupCriteria>()).Returns(new CommandResult());
            kpiGroupController = GetKPIGroupController(kpiGroupService, sessionService);

            // Act
            var actual = kpiGroupController.DeleteKPIGroup(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion DeleteKPIGroup

        #region Helpers

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private IKPIGroupService FakeKPIGroupService()
        {
            var service = Substitute.For<IKPIGroupService>();

            return service;
        }

        private KPIGroupController GetKPIGroupController(
            IKPIGroupService kpiGroupService = null,
            ISessionService sessionService = null)
        {
            var controller = new KPIGroupController(kpiGroupService);
            controller.SessionService = sessionService;

            return controller;
        }

        #endregion Helpers
    }
}