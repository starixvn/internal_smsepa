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
    public class KPIGroupControllerTests
    {
        private IKPIGroupService _kpiGroupService;
        private ICommonService commonService;
        private ISessionService _sessionService;
        private KPIGroupController _kpiGroupController;

        #region GetKPIGroup

        [Test]
        public void GetKPIGroup_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _kpiGroupController = GetKPIGroupController();

            // Act
            var actual = _kpiGroupController.GetType().GetMethod("GetKPIGroup");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetKPIGroup_WhenCalled_ReturnListOfKPIGroups()
        {
            // Arrange
            IEnumerable<KPIGroupModel> result = GetListOfKPIGroupForTest();
            _kpiGroupService = FakeKPIGroupService();
            _kpiGroupService.GetKPIGroup(Arg.Any<GetKPIGroupCriteria>()).Returns(result);
            _kpiGroupController = GetKPIGroupController(_kpiGroupService);

            // Act
            var actual = _kpiGroupController.GetKPIGroup(Arg.Any<bool?>());

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(result, actual);
        }

        #endregion GetKPIGroup

        #region GetKPIGroupById

        [Test]
        public void GetKPIGroupById_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _kpiGroupController = GetKPIGroupController();

            // Act
            var actual = _kpiGroupController.GetType().GetMethod("GetKPIGroupById");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetKPIGroupById_WhenCalled_ReturnExpectedKPIGroup()
        {
            // Arrange
            int kpiGroupId = 1;
            KPIGroupModel result = GetListOfKPIGroupForTest().First();
            _kpiGroupService = FakeKPIGroupService();
            _kpiGroupService.GetKPIGroupById(kpiGroupId).Returns(result);
            _kpiGroupController = GetKPIGroupController(_kpiGroupService);

            // Act
            var actual = _kpiGroupController.GetKPIGroupById(kpiGroupId);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(result, actual);
            Assert.AreEqual(kpiGroupId, actual.KPIGroupId);
        }

        #endregion GetKPIGroupById

        #region CreateKPIGroup

        [Test]
        public void CreateKPIGroup_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _kpiGroupController = GetKPIGroupController();

            // Act
            var actual = _kpiGroupController.GetType().GetMethod("CreateKPIGroup");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void CreateKPIGroup_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _kpiGroupService = FakeKPIGroupService();
            _sessionService = FakeSessionService();
            commonService = FakeCommonService();
            commonService.ExecuteNonQuery(Arg.Any<CreateKPIGroupCriteria>()).Returns(new CommandResult());
            _kpiGroupController = GetKPIGroupController(_kpiGroupService, _sessionService, commonService);

            // Act
            var actual = _kpiGroupController.CreateKPIGroup(Arg.Any<CreateKPIGroupCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion CreateKPIGroup

        #region UpdateKPIGroup

        [Test]
        public void UpdateKPIGroup_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _kpiGroupController = GetKPIGroupController();

            // Act
            var actual = _kpiGroupController.GetType().GetMethod("UpdateKPIGroup");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void UpdateKPIGroup_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _kpiGroupService = FakeKPIGroupService();
            _sessionService = FakeSessionService();
            commonService = FakeCommonService();
            commonService.ExecuteNonQuery(Arg.Any<UpdateKPIGroupCriteria>()).Returns(new CommandResult());
            _kpiGroupController = GetKPIGroupController(_kpiGroupService, _sessionService, commonService);

            // Act
            var actual = _kpiGroupController.UpdateKPIGroup(Arg.Any<UpdateKPIGroupCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateKPIGroup

        #region UpdateStatusKPIGroup

        [Test]
        public void UpdateStatusKPIGroup_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _kpiGroupController = GetKPIGroupController();

            // Act
            var actual = _kpiGroupController.GetType().GetMethod("UpdateStatusKPIGroup");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void UpdateStatusKPIGroup_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _kpiGroupService = FakeKPIGroupService();
            _sessionService = FakeSessionService();
            commonService = FakeCommonService();
            commonService.ExecuteNonQuery(Arg.Any<UpdateStatusKPIGroupCriteria>()).Returns(new CommandResult());
            _kpiGroupController = GetKPIGroupController(_kpiGroupService, _sessionService, commonService);

            // Act
            var actual = _kpiGroupController.UpdateStatusKPIGroup(Arg.Any<UpdateStatusKPIGroupCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateStatusKPIGroup

        #region DeleteKPIGroup

        [Test]
        public void DeleteKPIGroup_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _kpiGroupController = GetKPIGroupController();

            // Act
            var actual = _kpiGroupController.GetType().GetMethod("DeleteKPIGroup");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void DeleteKPIGroup_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _kpiGroupService = FakeKPIGroupService();
            _sessionService = FakeSessionService();
            commonService = FakeCommonService();
            commonService.ExecuteNonQuery(Arg.Any<DeleteKPIGroupCriteria>()).Returns(new CommandResult());
            _kpiGroupController = GetKPIGroupController(_kpiGroupService, _sessionService, commonService);

            // Act
            var actual = _kpiGroupController.DeleteKPIGroup(Arg.Any<int>());

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

        private ICommonService FakeCommonService()
        {
            var service = Substitute.For<ICommonService>();

            return service;
        }

        private IKPIGroupService FakeKPIGroupService()
        {
            var service = Substitute.For<IKPIGroupService>();

            return service;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Reviewed")]
        private KPIGroupController GetKPIGroupController(
            IKPIGroupService kpiGroupService = null,
            ISessionService sessionService = null,
            ICommonService commonService = null)
        {
            var controller = new KPIGroupController(kpiGroupService, commonService);
            controller.SessionService = sessionService;

            return controller;
        }

        private IEnumerable<KPIGroupModel> GetListOfKPIGroupForTest()
        {
            var kpiGroups = new List<KPIGroupModel>
            {
                new KPIGroupModel
                {
                    KPIGroupId = 1,
                    Name = "group test 1"
                },
                new KPIGroupModel
                {
                    KPIGroupId = 2,
                    Name = "group test 2"
                }
            };

            return kpiGroups;
        }

        #endregion Helpers
    }
}