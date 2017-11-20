namespace SMS.EPA.Site.UnitTests
{
    using System.Collections.Generic;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Model;
    using NSubstitute;
    using NUnit.Framework;
    using Services.Interfaces;
    using Site.Controllers.API;

    [TestFixture]
    public class KPIControllerTests
    {
        private IKPIService _kpiService;
        private IBaseService baseService;
        private ISessionService _sessionService;
        private KPIController _kpiController;

        #region GetKPI

        [Test]
        public void GetKPI_WhenCalled_ReturnExpectedKPIs()
        {
            // Arrange
            var kpis = new List<KPIModel>();
            _kpiService = FakeKPIService();
            _kpiService.GetKPI(Arg.Any<GetKPICriteria>()).Returns(kpis);
            _kpiController = GetKPIController(_kpiService);

            // Act
            var actual = _kpiController.GetKPI(Arg.Any<int?>(), Arg.Any<bool?>(), Arg.Any<bool?>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<KPIModel>), actual);
            Assert.AreEqual(kpis, actual);
        }

        #endregion GetKPI

        #region CreateKPI

        [Test]
        public void CreateKPI_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            _kpiService = FakeKPIService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<CreateKPICriteria>()).Returns(expected);
            _kpiController = GetKPIController(_kpiService, _sessionService, baseService);

            // Act
            var actual = _kpiController.CreateKPI(Arg.Any<CreateKPICriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion CreateKPI

        #region UpdateKPI

        [Test]
        public void UpdateKPI_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            _kpiService = FakeKPIService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<UpdateKPICriteria>()).Returns(expected);
            _kpiController = GetKPIController(_kpiService, _sessionService, baseService);

            // Act
            var actual = _kpiController.UpdateKPI(Arg.Any<UpdateKPICriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion UpdateKPI

        #region UpdateStatusKPI

        [Test]
        public void UpdateStatusKPI_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            _kpiService = FakeKPIService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<UpdateStatusKPICriteria>()).Returns(expected);
            _kpiController = GetKPIController(_kpiService, _sessionService, baseService);

            // Act
            var actual = _kpiController.UpdateStatusKPI(Arg.Any<UpdateStatusKPICriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion UpdateStatusKPI

        #region UpdateRequiredEvaluateKPI

        [Test]
        public void UpdateRequiredEvaluateKPI_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            _kpiService = FakeKPIService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<UpdateRequiredEvaluateKPICriteria>()).Returns(expected);
            _kpiController = GetKPIController(_kpiService, _sessionService, baseService);

            // Act
            var actual = _kpiController.UpdateRequiredEvaluateKPI(Arg.Any<UpdateRequiredEvaluateKPICriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion UpdateRequiredEvaluateKPI

        #region DeleteKPI

        [Test]
        public void DeleteKPI_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            _kpiService = FakeKPIService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<DeleteKPICriteria>()).Returns(expected);
            _kpiController = GetKPIController(_kpiService, _sessionService, baseService);

            // Act
            var actual = _kpiController.DeleteKPI(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion DeleteKPI

        #region Helpers

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private IKPIService FakeKPIService()
        {
            var service = Substitute.For<IKPIService>();

            return service;
        }

        private KPIController GetKPIController(
            IKPIService kpiService = null,
            ISessionService sessionService = null,
            IBaseService baseService = null)
        {
            var controller = new KPIController(kpiService, baseService) { SessionService = sessionService };

            return controller;
        }

        #endregion Helpers
    }
}