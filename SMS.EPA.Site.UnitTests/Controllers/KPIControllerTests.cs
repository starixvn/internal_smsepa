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
        public void GetKPI_ValidActionName_ReturnAsExpected()
        {
            // Arrange
            kpiController = GetKPIController();

            // Act
            var actual = kpiController.GetType().GetMethod("GetKPI");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetKPI_WhenCalled_ReturnsListOfKPIModel()
        {
            // Arrange
            kpiService = FakeKPIService();
            sessionService = FakeSessionService();
            var expected = new List<KPIModel>();
            kpiService.GetKPI(Arg.Is<GetKPICriteria>(criteria => criteria != null)).Returns(expected);
            kpiController = GetKPIController(kpiService, sessionService);

            // Act
            var actual = kpiController.GetKPI(Arg.Any<int?>(), Arg.Any<bool?>(), Arg.Any<bool?>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<KPIModel>), actual);
        }

        #endregion GetKPI

        #region CreateKPI

        [Test]
        public void CreateKPI_ValidActionName_ReturnAsExpected()
        {
            // Arrange
            kpiController = GetKPIController();

            // Act
            var actual = kpiController.GetType().GetMethod("CreateKPI");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void CreateKPI_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            kpiService = FakeKPIService();
            sessionService = FakeSessionService();
            var expected = new CommandResult();
            kpiService.CreateKPI(Arg.Is<CreateKPICriteria>(criteria => criteria != null)).Returns(expected);
            kpiController = GetKPIController(kpiService, sessionService);

            // Act
            var actual = kpiController.CreateKPI(new CreateKPICriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion CreateKPI

        #region UpdateKPI

        [Test]
        public void UpdateKPI_ValidActionName_ReturnAsExpected()
        {
            // Arrange
            kpiController = GetKPIController();

            // Act
            var actual = kpiController.GetType().GetMethod("UpdateKPI");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void UpdateKPI_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            kpiService = FakeKPIService();
            sessionService = FakeSessionService();
            var expected = new CommandResult();
            kpiService.UpdateKPI(Arg.Is<UpdateKPICriteria>(criteria => criteria != null)).Returns(expected);
            kpiController = GetKPIController(kpiService, sessionService);

            // Act
            var actual = kpiController.UpdateKPI(new UpdateKPICriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateKPI

        #region UpdateStatusKPI

        [Test]
        public void UpdateStatusKPI_ValidActionName_ReturnAsExpected()
        {
            // Arrange
            kpiController = GetKPIController();

            // Act
            var actual = kpiController.GetType().GetMethod("UpdateStatusKPI");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void UpdateStatusKPI_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            kpiService = FakeKPIService();
            sessionService = FakeSessionService();
            var expected = new CommandResult();
            kpiService.UpdateStatusKPI(Arg.Is<UpdateStatusKPICriteria>(criteria => criteria != null)).Returns(expected);
            kpiController = GetKPIController(kpiService, sessionService);

            // Act
            var actual = kpiController.UpdateStatusKPI(new UpdateStatusKPICriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateStatusKPI

        #region UpdateRequiredEvaluateKPI

        [Test]
        public void UpdateRequiredEvaluateKPI_ValidActionName_ReturnAsExpected()
        {
            // Arrange
            kpiController = GetKPIController();

            // Act
            var actual = kpiController.GetType().GetMethod("UpdateRequiredEvaluateKPI");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void UpdateRequiredEvaluateKPI_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            kpiService = FakeKPIService();
            sessionService = FakeSessionService();
            kpiController = GetKPIController(kpiService, sessionService);

            var expected = new CommandResult();
            kpiService.UpdateRequiredEvaluateKPI(Arg.Is<UpdateRequiredEvaluateKPICriteria>(criteria => criteria != null)).Returns(expected);

            // Act
            var actual = kpiController.UpdateRequiredEvaluateKPI(new UpdateRequiredEvaluateKPICriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion UpdateRequiredEvaluateKPI

        #region DeleteKPI

        [Test]
        public void DeleteKPI_ValidActionName_ReturnAsExpected()
        {
            // Arrange
            kpiController = GetKPIController();

            // Act
            var actual = kpiController.GetType().GetMethod("DeleteKPI");

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void DeleteKPI_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            kpiService = FakeKPIService();
            sessionService = FakeSessionService();
            var expected = new CommandResult();
            kpiService.DeleteKPI(Arg.Is<DeleteKPICriteria>(criteria => criteria != null)).Returns(expected);
            kpiController = GetKPIController(kpiService, sessionService);

            // Act
            var actual = kpiController.DeleteKPI(Arg.Is<int>(x => x > 0));

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #endregion DeleteKPI

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
    }
}