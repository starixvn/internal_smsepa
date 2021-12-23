namespace SMS.EPA.Site.UnitTests
{
    using System.Collections.Generic;
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
    public class KPIControllerTests
    {
        private IKPIService kpiService;
        private IBaseService baseService;
        private IUserControlSession userControlSession;

        #region GetKPI

        [Test]
        public void GetKPI_WhenCalled_ReturnExpectedKPIs()
        {
            // Arrange
            var kpis = new List<KPIModel>();
            var controller = GetKPIController();
            kpiService.GetKPI(Arg.Any<GetKPICriteria>()).Returns(kpis);

            // Act
            var actual = controller.GetKPI(Arg.Any<int?>(), Arg.Any<bool?>(), Arg.Any<bool?>());

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
            var controller = GetKPIController();
            baseService.ExecuteNonQuery(Arg.Any<CreateKPICriteria>()).Returns(expected);

            // Act
            var actual = controller.CreateKPI(Arg.Any<CreateKPICriteria>());

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
            var controller = GetKPIController();
            baseService.ExecuteNonQuery(Arg.Any<UpdateKPICriteria>()).Returns(expected);

            // Act
            var actual = controller.UpdateKPI(Arg.Any<UpdateKPICriteria>());

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
            var controller = GetKPIController();
            baseService.ExecuteNonQuery(Arg.Any<UpdateStatusKPICriteria>()).Returns(expected);

            // Act
            var actual = controller.UpdateStatusKPI(Arg.Any<UpdateStatusKPICriteria>());

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
            var controller = GetKPIController();
            baseService.ExecuteNonQuery(Arg.Any<UpdateRequiredEvaluateKPICriteria>()).Returns(expected);

            // Act
            var actual = controller.UpdateRequiredEvaluateKPI(Arg.Any<UpdateRequiredEvaluateKPICriteria>());

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
            var controller = GetKPIController();
            baseService.ExecuteNonQuery(Arg.Any<DeleteKPICriteria>()).Returns(expected);

            // Act
            var actual = controller.DeleteKPI(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion DeleteKPI

        #region Helpers

        private static IUserControlSession FakeUserControlSession()
        {
            var session = Substitute.For<IUserControlSession>();
            session.Set<int>(Constants.CurrentEmployeeIdKey, 1);

            return session;
        }

        private KPIController GetKPIController()
        {
            userControlSession = FakeUserControlSession();
            kpiService = Substitute.For<IKPIService>();
            baseService = Substitute.For<IBaseService>();
            var controller = new KPIController(kpiService, baseService)
            {
                UserControlSession = userControlSession
            };

            return controller;
        }

        #endregion Helpers
    }
}