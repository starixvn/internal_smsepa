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
        public void GetKPI_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _kpiController = GetKPIController();

            // Act
            var actual = _kpiController.GetType().GetMethod("GetKPI");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetKPI_WhenCalled_ReturnInstanceOfListOfKPIModel()
        {
            // Arrange
            _kpiService = FakeKPIService();
            _kpiService.GetKPI(Arg.Any<GetKPICriteria>()).Returns(new List<KPIModel>());
            _kpiController = GetKPIController(_kpiService);

            // Act
            var actual = _kpiController.GetKPI(Arg.Any<int?>(), Arg.Any<bool?>(), Arg.Any<bool?>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<KPIModel>), actual);
        }

        #endregion GetKPI

        #region CreateKPI

        [Test]
        public void CreateKPI_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _kpiController = GetKPIController();

            // Act
            var actual = _kpiController.GetType().GetMethod("CreateKPI");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void CreateKPI_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _kpiService = FakeKPIService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<CreateKPICriteria>()).Returns(new CommandResult());
            _kpiController = GetKPIController(_kpiService, _sessionService, baseService);

            // Act
            var actual = _kpiController.CreateKPI(Arg.Any<CreateKPICriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion CreateKPI

        #region UpdateKPI

        [Test]
        public void UpdateKPI_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _kpiController = GetKPIController();

            // Act
            var actual = _kpiController.GetType().GetMethod("UpdateKPI");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void UpdateKPI_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _kpiService = FakeKPIService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<UpdateKPICriteria>()).Returns(new CommandResult());
            _kpiController = GetKPIController(_kpiService, _sessionService, baseService);

            // Act
            var actual = _kpiController.UpdateKPI(Arg.Any<UpdateKPICriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateKPI

        #region UpdateStatusKPI

        [Test]
        public void UpdateStatusKPI_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _kpiController = GetKPIController();

            // Act
            var actual = _kpiController.GetType().GetMethod("UpdateStatusKPI");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void UpdateStatusKPI_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _kpiService = FakeKPIService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<UpdateStatusKPICriteria>()).Returns(new CommandResult());
            _kpiController = GetKPIController(_kpiService, _sessionService, baseService);

            // Act
            var actual = _kpiController.UpdateStatusKPI(Arg.Any<UpdateStatusKPICriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateStatusKPI

        #region UpdateRequiredEvaluateKPI

        [Test]
        public void UpdateRequiredEvaluateKPI_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _kpiController = GetKPIController();

            // Act
            var actual = _kpiController.GetType().GetMethod("UpdateRequiredEvaluateKPI");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void UpdateRequiredEvaluateKPI_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _kpiService = FakeKPIService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<UpdateRequiredEvaluateKPICriteria>()).Returns(new CommandResult());
            _kpiController = GetKPIController(_kpiService, _sessionService, baseService);

            // Act
            var actual = _kpiController.UpdateRequiredEvaluateKPI(Arg.Any<UpdateRequiredEvaluateKPICriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateRequiredEvaluateKPI

        #region DeleteKPI

        [Test]
        public void DeleteKPI_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            _kpiController = GetKPIController();

            // Act
            var actual = _kpiController.GetType().GetMethod("DeleteKPI");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void DeleteKPI_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _kpiService = FakeKPIService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<DeleteKPICriteria>()).Returns(new CommandResult());
            _kpiController = GetKPIController(_kpiService, _sessionService, baseService);

            // Act
            var actual = _kpiController.DeleteKPI(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion DeleteKPI

        #region Helpers

        private ISessionService FakeSessionService()
        {
            ISessionService service = Substitute.For<ISessionService>();
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
            IKPIService service = Substitute.For<IKPIService>();

            return service;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Reviewed")]
        private KPIController GetKPIController(
            IKPIService kpiService = null,
            ISessionService sessionService = null,
            IBaseService baseService = null)
        {
            var controller = new KPIController(kpiService, baseService);
            controller.SessionService = sessionService;

            return controller;
        }

        #endregion Helpers
    }
}