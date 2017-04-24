namespace SMS.EPA.Site.UnitTests
{
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Model;
    using NSubstitute;
    using NUnit.Framework;
    using Services.Interfaces;
    using Site.Controllers.API;

    [TestFixture]
    public class MonthlyPeriodControllerTests
    {
        private IPeriodService _periodService;
        private ISessionService _sessionService;
        private MonthlyPeriodController _monthlyPeriodController;

        [Test]
        public void GetMonthlyPeriodById_WhenCalled_ReturnExpectedPeriod()
        {
            // Arrange
            int periodId = 1;
            var period = new PeriodModel
            {
                PeriodId = periodId,
                Name = "test 1"
            };

            _periodService = FakePeriodService();
            _periodService.GetPeriodById(periodId).Returns(period);
            _monthlyPeriodController = GetMonthlyPeriodController(_periodService);

            // Act
            var actual = _monthlyPeriodController.GetMonthlyPeriodById(periodId);

            // Assert
            Assert.AreEqual(period, actual);
            Assert.AreEqual(periodId, actual.PeriodId);
        }

        [Test]
        public void CreateMonthlyPeriod_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _periodService = FakePeriodService();
            _sessionService = FakeSessionService();
            _periodService.CreatePeriod(Arg.Any<CreatePeriodCriteria>()).Returns(new CommandResult());
            _monthlyPeriodController = GetMonthlyPeriodController(_periodService, _sessionService);

            // Act
            var actual = _monthlyPeriodController.CreateMonthlyPeriod(Arg.Any<CreatePeriodCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateBasicInfoMonthlyPeriod_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            _periodService = FakePeriodService();
            _sessionService = FakeSessionService();
            _periodService.UpdateBasicInformationPeriod(Arg.Any<UpdateBasicInformationPeriodCriteria>()).Returns(new CommandResult());
            _monthlyPeriodController = GetMonthlyPeriodController(_periodService, _sessionService);

            // Act
            var actual = _monthlyPeriodController.UpdateBasicInfoMonthlyPeriod(Arg.Any<UpdateBasicInformationPeriodCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        #region Helpers

        private ISessionService FakeSessionService()
        {
            ISessionService service = Substitute.For<ISessionService>();
            service.EmployeeId = 1;

            return service;
        }

        private IPeriodService FakePeriodService()
        {
            IPeriodService service = Substitute.For<IPeriodService>();

            return service;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Reviewed")]
        private MonthlyPeriodController GetMonthlyPeriodController(IPeriodService periodService = null, ISessionService sessionService = null)
        {
            var controller = new MonthlyPeriodController(periodService);
            controller.SessionService = sessionService;

            return controller;
        }

        #endregion Helpers
    }
}