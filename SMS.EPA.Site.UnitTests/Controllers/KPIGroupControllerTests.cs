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
    using Site.Controllers.API;

    [TestFixture]
    public class KPIGroupControllerTests
    {
        private IBaseService baseService;
        private ISessionService _sessionService;
        private KPIGroupController _kpiGroupController;

        [Test]
        public void GetKPIGroup_WhenCalled_ReturnListOfKPIGroups()
        {
            // Arrange
            IEnumerable<KPIGroupModel> result = GetListOfKPIGroupForTest();
            baseService = FakeBaseService();
            baseService.GetData<KPIGroupModel>(Arg.Any<GetKPIGroupCriteria>()).Returns(result);
            _kpiGroupController = GetKPIGroupController(null, baseService);

            // Act
            var actual = _kpiGroupController.GetKPIGroup(Arg.Any<bool?>());

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(result, actual);
        }

        [Test]
        public void GetKPIGroupById_WhenCalled_ReturnExpectedKPIGroup()
        {
            // Arrange
            int kpiGroupId = 1;
            KPIGroupModel result = GetListOfKPIGroupForTest().First();
            baseService = FakeBaseService();
            baseService.GetDataEntity<KPIGroupModel>(Arg.Any<GetKPIGroupByIdCriteria>()).Returns(result);
            _kpiGroupController = GetKPIGroupController(null, baseService);

            // Act
            var actual = _kpiGroupController.GetKPIGroupById(kpiGroupId);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(result, actual);
        }

        [Test]
        public void CreateKPIGroup_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<CreateKPIGroupCriteria>()).Returns(expected);
            _kpiGroupController = GetKPIGroupController(_sessionService, baseService);

            // Act
            var actual = _kpiGroupController.CreateKPIGroup(Arg.Any<CreateKPIGroupCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UpdateKPIGroup_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<UpdateKPIGroupCriteria>()).Returns(expected);
            _kpiGroupController = GetKPIGroupController(_sessionService, baseService);

            // Act
            var actual = _kpiGroupController.UpdateKPIGroup(Arg.Any<UpdateKPIGroupCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UpdateStatusKPIGroup_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<UpdateStatusKPIGroupCriteria>()).Returns(expected);
            _kpiGroupController = GetKPIGroupController(_sessionService, baseService);

            // Act
            var actual = _kpiGroupController.UpdateStatusKPIGroup(Arg.Any<UpdateStatusKPIGroupCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DeleteKPIGroup_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<DeleteKPIGroupCriteria>()).Returns(expected);
            _kpiGroupController = GetKPIGroupController(_sessionService, baseService);

            // Act
            var actual = _kpiGroupController.DeleteKPIGroup(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

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

        private KPIGroupController GetKPIGroupController(ISessionService sessionService = null, IBaseService baseService = null)
        {
            var controller = new KPIGroupController(baseService) { SessionService = sessionService };

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