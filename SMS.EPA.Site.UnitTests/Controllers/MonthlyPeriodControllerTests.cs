namespace SMS.EPA.Site.UnitTests
{
    using System.Diagnostics.CodeAnalysis;
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
        private IBaseService baseService;
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
        public void CreateMonthlyPeriod_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            _periodService = FakePeriodService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<CreatePeriodCriteria>()).Returns(expected);
            _monthlyPeriodController = GetMonthlyPeriodController(_periodService, _sessionService, baseService);

            // Act
            var actual = _monthlyPeriodController.CreateMonthlyPeriod(new CreatePeriodCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UpdateBasicInfoMonthlyPeriod_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            _periodService = FakePeriodService();
            _sessionService = FakeSessionService();
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<UpdateBasicInformationPeriodCriteria>()).Returns(expected);
            _monthlyPeriodController = GetMonthlyPeriodController(_periodService, _sessionService, baseService);

            // Act
            var actual = _monthlyPeriodController.UpdateBasicInfoMonthlyPeriod(new UpdateBasicInformationPeriodCriteria());

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

        private static IPeriodService FakePeriodService()
        {
            var service = Substitute.For<IPeriodService>();

            return service;
        }

        private static IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Reviewed")]
        private MonthlyPeriodController GetMonthlyPeriodController(
            IPeriodService periodService = null,
            ISessionService sessionService = null,
            IBaseService baseService = null)
        {
            var controller = new MonthlyPeriodController(periodService, baseService) { SessionService = sessionService };

            return controller;
        }

        #endregion Helpers
    }
}