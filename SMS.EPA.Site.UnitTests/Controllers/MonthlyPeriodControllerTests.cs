namespace SMS.EPA.Site.UnitTests
{
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Interfaces;
    using SMS.EPA.Site.Controllers.API;

    [TestFixture]
    public class MonthlyPeriodControllerTests
    {
        private IPeriodService periodService;
        private ISessionService sessionService;
        private MonthlyPeriodController monthlyPeriodController;

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

            periodService = FakePeriodService();
            periodService.GetPeriodById(Arg.Any<int>()).Returns(period);
            monthlyPeriodController = GetMonthlyPeriodController(periodService);

            // Act
            var actual = monthlyPeriodController.GetMonthlyPeriodById(periodId);

            // Assert
            Assert.AreEqual(period, actual);
            Assert.AreEqual(periodId, actual.PeriodId);
        }

        [Test]
        public void CreateMonthlyPeriod_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            periodService = FakePeriodService();
            sessionService = FakeSessionService();
            periodService.CreatePeriod(Arg.Any<CreatePeriodCriteria>()).Returns(new CommandResult());
            monthlyPeriodController = GetMonthlyPeriodController(periodService, sessionService);

            // Act
            var actual = monthlyPeriodController.CreateMonthlyPeriod(Arg.Any<CreatePeriodCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateBasicInfoMonthlyPeriod_WhenCalled_ReturnInstanceOfCommandResult()
        {
            // Arrange
            periodService = FakePeriodService();
            sessionService = FakeSessionService();
            periodService.UpdateBasicInformationPeriod(Arg.Any<UpdateBasicInformationPeriodCriteria>()).Returns(new CommandResult());
            monthlyPeriodController = GetMonthlyPeriodController(periodService, sessionService);

            // Act
            var actual = monthlyPeriodController.UpdateBasicInfoMonthlyPeriod(Arg.Any<UpdateBasicInformationPeriodCriteria>());

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

        private MonthlyPeriodController GetMonthlyPeriodController(IPeriodService periodService = null, ISessionService sessionService = null)
        {
            var controller = new MonthlyPeriodController(periodService);
            controller.SessionService = sessionService;

            return controller;
        }

        #endregion Helpers
    }
}