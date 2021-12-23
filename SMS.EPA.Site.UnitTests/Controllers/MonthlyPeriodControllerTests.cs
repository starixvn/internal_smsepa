namespace SMS.EPA.Site.UnitTests
{
    using Core.Model;
    using Core.Services;
    using Criteria;
    using HRM.UserControl.Client.Core.Session;
    using Model;
    using NSubstitute;
    using NUnit.Framework;
    using Site.Controllers.API;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Services;

    [TestFixture]
    public class MonthlyPeriodControllerTests
    {
        private IPeriodService periodService;
        private IBaseService baseService;
        private IUserControlSession userControlSession;

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
            var controller = GetMonthlyPeriodController();
            periodService.GetPeriodById(periodId).Returns(period);

            // Act
            var actual = controller.GetMonthlyPeriodById(periodId);

            // Assert
            Assert.AreEqual(period, actual);
            Assert.AreEqual(periodId, actual.PeriodId);
        }

        [Test]
        public void CreateMonthlyPeriod_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetMonthlyPeriodController();
            baseService.ExecuteNonQuery(Arg.Any<CreatePeriodCriteria>()).Returns(expected);

            // Act
            var actual = controller.CreateMonthlyPeriod(new CreatePeriodCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UpdateBasicInfoMonthlyPeriod_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetMonthlyPeriodController();
            baseService.ExecuteNonQuery(Arg.Any<UpdateBasicInformationPeriodCriteria>()).Returns(expected);

            // Act
            var actual = controller.UpdateBasicInfoMonthlyPeriod(new UpdateBasicInformationPeriodCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        #region Helpers

        private static IUserControlSession FakeUserControlSession()
        {
            var session = Substitute.For<IUserControlSession>();
            session.Set<int>(Constants.CurrentEmployeeIdKey, 1);

            return session;
        }

        private MonthlyPeriodController GetMonthlyPeriodController()
        {
            userControlSession = FakeUserControlSession();
            periodService = Substitute.For<IPeriodService>();
            baseService = Substitute.For<IBaseService>();
            var controller = new MonthlyPeriodController(periodService, baseService)
            {
                UserControlSession = userControlSession
            };

            return controller;
        }

        #endregion Helpers
    }
}