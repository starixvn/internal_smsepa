namespace SMS.EPA.Site.UnitTests
{
    using System.Collections.Generic;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services.Interfaces;
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
        public void GetKPIGroup_ValidActionName_ReturnAsExpected()
        {
            // Arrange
            kpiGroupService = FakeKPIGroupService();
            sessionService = FakeSessionService();
            kpiGroupController = GetKPIGroupController(kpiGroupService, sessionService);

            // Act
            var actual = kpiGroupController.GetType().GetMethod("GetKPIGroup");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetKPIGroup_WhenCalled_ReturnsListOfKPIGroupModel()
        {
            // Arrange
            kpiGroupService = FakeKPIGroupService();
            sessionService = FakeSessionService();
            var expected = new List<KPIGroupModel>();
            kpiGroupService.GetKPIGroup(Arg.Is<GetKPIGroupCriteria>(criteria => criteria != null)).Returns(expected);
            kpiGroupController = GetKPIGroupController(kpiGroupService, sessionService);

            // Act
            var actual = kpiGroupController.GetKPIGroup(Arg.Any<bool?>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<KPIGroupModel>), actual);
        }

        #endregion GetKPIGroup

        #region GetKPIGroupById

        [Test]
        public void GetKPIGroupById_ValidActionName_ReturnAsExpected()
        {
            // Arrange
            kpiGroupService = FakeKPIGroupService();
            sessionService = FakeSessionService();
            kpiGroupController = GetKPIGroupController(kpiGroupService, sessionService);

            // Act
            var actual = kpiGroupController.GetType().GetMethod("GetKPIGroupById");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetKPIGroupById_WhenCalled_ReturnsKPIGroupModel()
        {
            // Arrange
            kpiGroupService = FakeKPIGroupService();
            sessionService = FakeSessionService();
            var expected = new KPIGroupModel();
            kpiGroupService.GetKPIGroupById(Arg.Is<GetKPIGroupByIdCriteria>(criteria => criteria != null)).Returns(expected);
            kpiGroupController = GetKPIGroupController(kpiGroupService, sessionService);

            // Act
            var actual = kpiGroupController.GetKPIGroupById(Arg.Is<int>(x => x > 0));

            // Assert
            Assert.IsInstanceOf(typeof(KPIGroupModel), actual);
        }

        #endregion GetKPIGroupById

        #region CreateKPIGroup

        [Test]
        public void CreateKPIGroup_ValidActionName_ReturnAsExpected()
        {
            // Arrange
            kpiGroupService = FakeKPIGroupService();
            sessionService = FakeSessionService();
            kpiGroupController = GetKPIGroupController(kpiGroupService, sessionService);

            // Act
            var actual = kpiGroupController.GetType().GetMethod("CreateKPIGroup");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void CreateKPIGroup_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            kpiGroupService = FakeKPIGroupService();
            sessionService = FakeSessionService();
            var expected = new CommandResult();
            kpiGroupService.CreateKPIGroup(Arg.Is<CreateKPIGroupCriteria>(criteria => criteria != null)).Returns(expected);
            kpiGroupController = GetKPIGroupController(kpiGroupService, sessionService);

            // Act
            var actual = kpiGroupController.CreateKPIGroup(new CreateKPIGroupCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion CreateKPIGroup

        #region UpdateKPIGroup

        [Test]
        public void UpdateKPIGroup_ValidActionName_ReturnAsExpected()
        {
            // Arrange
            kpiGroupService = FakeKPIGroupService();
            sessionService = FakeSessionService();
            kpiGroupController = GetKPIGroupController(kpiGroupService, sessionService);

            // Act
            var actual = kpiGroupController.GetType().GetMethod("UpdateKPIGroup");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void UpdateKPIGroup_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            kpiGroupService = FakeKPIGroupService();
            sessionService = FakeSessionService();
            var expected = new CommandResult();
            kpiGroupService.UpdateKPIGroup(Arg.Is<UpdateKPIGroupCriteria>(criteria => criteria != null)).Returns(expected);
            kpiGroupController = GetKPIGroupController(kpiGroupService, sessionService);

            // Act
            var actual = kpiGroupController.UpdateKPIGroup(new UpdateKPIGroupCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateKPIGroup

        #region UpdateStatusKPIGroup

        [Test]
        public void UpdateStatusKPIGroup_ValidActionName_ReturnAsExpected()
        {
            // Arrange
            kpiGroupService = FakeKPIGroupService();
            sessionService = FakeSessionService();
            kpiGroupController = GetKPIGroupController(kpiGroupService, sessionService);

            // Act
            var actual = kpiGroupController.GetType().GetMethod("UpdateStatusKPIGroup");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void UpdateStatusKPIGroup_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            kpiGroupService = FakeKPIGroupService();
            sessionService = FakeSessionService();
            var expected = new CommandResult();
            kpiGroupService.UpdateStatusKPIGroup(Arg.Is<UpdateStatusKPIGroupCriteria>(criteria => criteria != null)).Returns(expected);
            kpiGroupController = GetKPIGroupController(kpiGroupService, sessionService);

            // Act
            var actual = kpiGroupController.UpdateStatusKPIGroup(new UpdateStatusKPIGroupCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateStatusKPIGroup

        #region DeleteKPIGroup

        [Test]
        public void DeleteKPIGroup_ValidActionName_ReturnAsExpected()
        {
            // Arrange
            kpiGroupService = FakeKPIGroupService();
            sessionService = FakeSessionService();
            kpiGroupController = GetKPIGroupController(kpiGroupService, sessionService);

            // Act
            var actual = kpiGroupController.GetType().GetMethod("DeleteKPIGroup");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void DeleteKPIGroup_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            kpiGroupService = FakeKPIGroupService();
            sessionService = FakeSessionService();
            var expected = new CommandResult();
            kpiGroupService.DeleteKPIGroup(Arg.Is<DeleteKPIGroupCriteria>(criteria => criteria != null)).Returns(expected);
            kpiGroupController = GetKPIGroupController(kpiGroupService, sessionService);

            // Act
            var actual = kpiGroupController.DeleteKPIGroup(Arg.Is<int>(x => x > 0));

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion DeleteKPIGroup

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

        private KPIGroupController GetKPIGroupController(IKPIGroupService kpiGroupService, ISessionService sessionService)
        {
            var controller = new KPIGroupController(kpiGroupService);
            controller.SessionService = sessionService;

            return controller;
        }
    }
}