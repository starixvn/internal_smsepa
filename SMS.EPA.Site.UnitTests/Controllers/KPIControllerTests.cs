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
    public class KPIControllerTests
    {
        private IKPIService kpiService;
        private ISessionService sessionService;
        private KPIController kpiController;

        #region GetKPI

        [Test]
        public void GetKPI_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            kpiController = GetKPIController();

            // Act
            var actual = kpiController.GetType().GetMethod("GetKPI");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetKPI_WhenCalled_ReturnInstanceOfListOfKPIModel()
        {
            // Arrange
            kpiService = FakeKPIService();
            kpiService.GetKPI(Arg.Any<GetKPICriteria>()).Returns(new List<KPIModel>());
            kpiController = GetKPIController(kpiService);

            // Act
            var actual = kpiController.GetKPI(Arg.Any<int?>(), Arg.Any<bool?>(), Arg.Any<bool?>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<KPIModel>), actual);
        }

        #endregion GetKPI

        #region CreateKPI

        [Test]
        public void CreateKPI_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            kpiController = GetKPIController();

            // Act
            var actual = kpiController.GetType().GetMethod("CreateKPI");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void CreateKPI_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            kpiService = FakeKPIService();
            sessionService = FakeSessionService();
            kpiService.CreateKPI(Arg.Any<CreateKPICriteria>()).Returns(new CommandResult());
            kpiController = GetKPIController(kpiService, sessionService);

            // Act
            var actual = kpiController.CreateKPI(Arg.Any<CreateKPICriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion CreateKPI

        #region UpdateKPI

        [Test]
        public void UpdateKPI_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            kpiController = GetKPIController();

            // Act
            var actual = kpiController.GetType().GetMethod("UpdateKPI");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void UpdateKPI_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            kpiService = FakeKPIService();
            sessionService = FakeSessionService();
            kpiService.UpdateKPI(Arg.Any<UpdateKPICriteria>()).Returns(new CommandResult());
            kpiController = GetKPIController(kpiService, sessionService);

            // Act
            var actual = kpiController.UpdateKPI(Arg.Any<UpdateKPICriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateKPI

        #region UpdateStatusKPI

        [Test]
        public void UpdateStatusKPI_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            kpiController = GetKPIController();

            // Act
            var actual = kpiController.GetType().GetMethod("UpdateStatusKPI");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void UpdateStatusKPI_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            kpiService = FakeKPIService();
            sessionService = FakeSessionService();
            kpiService.UpdateStatusKPI(Arg.Any<UpdateStatusKPICriteria>()).Returns(new CommandResult());
            kpiController = GetKPIController(kpiService, sessionService);

            // Act
            var actual = kpiController.UpdateStatusKPI(Arg.Any<UpdateStatusKPICriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateStatusKPI

        #region UpdateRequiredEvaluateKPI

        [Test]
        public void UpdateRequiredEvaluateKPI_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            kpiController = GetKPIController();

            // Act
            var actual = kpiController.GetType().GetMethod("UpdateRequiredEvaluateKPI");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void UpdateRequiredEvaluateKPI_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            kpiService = FakeKPIService();
            sessionService = FakeSessionService();
            kpiController = GetKPIController(kpiService, sessionService);
            kpiService.UpdateRequiredEvaluateKPI(Arg.Any<UpdateRequiredEvaluateKPICriteria>()).Returns(new CommandResult());

            // Act
            var actual = kpiController.UpdateRequiredEvaluateKPI(Arg.Any<UpdateRequiredEvaluateKPICriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateRequiredEvaluateKPI

        #region DeleteKPI

        [Test]
        public void DeleteKPI_CheckActionName_ReturnAsExpected()
        {
            // Arrange
            kpiController = GetKPIController();

            // Act
            var actual = kpiController.GetType().GetMethod("DeleteKPI");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void DeleteKPI_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            kpiService = FakeKPIService();
            sessionService = FakeSessionService();
            kpiService.DeleteKPI(Arg.Any<DeleteKPICriteria>()).Returns(new CommandResult());
            kpiController = GetKPIController(kpiService, sessionService);

            // Act
            var actual = kpiController.DeleteKPI(Arg.Any<int>());

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

        private IKPIService FakeKPIService()
        {
            IKPIService service = Substitute.For<IKPIService>();

            return service;
        }

        private KPIController GetKPIController(IKPIService kpiService = null, ISessionService sessionService = null)
        {
            var controller = new KPIController(kpiService);
            controller.SessionService = sessionService;

            return controller;
        }

        #endregion Helpers
    }
}