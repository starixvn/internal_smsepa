namespace SMS.EPA.Site.UnitTests.Controllers
{
    using System.Collections.Generic;
    using HRM.UserControl.Client.Core.Session;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Criteria.JobLevelStandard.KPI;
    using SMS.EPA.Model.JobLevelStandard;
    using SMS.EPA.Site.Controllers.API.JobLevelStandard;

    [TestFixture]
    public class JobLevelStandardKPIControllerTests
    {
        private IBaseService baseService;
        private IUserControlSession userControlSession;

        #region GetKPI

        [Test]
        public void GetKPI_WhenCalled_ReturnExpectedKPIs()
        {
            // Arrange
            var kpis = new List<JobLevelStandardKPI>();
            var controller = GetKPIController();
            baseService.GetData<JobLevelStandardKPI>(Arg.Any<GetJobLevelStandardKPICriteria>()).Returns(kpis);

            // Act
            var actual = controller.GetJobLevelStandardKPI(Arg.Any<GetJobLevelStandardKPICriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<JobLevelStandardKPI>), actual);
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
            baseService.ExecuteNonQuery(Arg.Any<CreateJobLevelStandardKPICriteria>()).Returns(expected);

            // Act
            var actual = controller.CreateJobLevelStandardKPI(Arg.Any<CreateJobLevelStandardKPICriteria>());

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
            baseService.ExecuteNonQuery(Arg.Any<UpdateJobLevelStandardKPICriteria>()).Returns(expected);

            // Act
            var actual = controller.UpdateJobLevelStandardKPI(Arg.Any<UpdateJobLevelStandardKPICriteria>());

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
            baseService.ExecuteNonQuery(Arg.Any<UpdateStatusJobLevelStandardKPICriteria>()).Returns(expected);

            // Act
            var actual = controller.UpdateStatusJobLevelStandardKPI(Arg.Any<UpdateStatusJobLevelStandardKPICriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion UpdateStatusKPI

        #region DeleteKPI

        [Test]
        public void DeleteKPI_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetKPIController();
            baseService.ExecuteNonQuery(Arg.Any<DeleteJobLevelStandardKPICriteria>()).Returns(expected);

            // Act
            var actual = controller.DeleteStatusJobLevelStandardKPI(Arg.Any<int>());

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
        private JobLevelStandardKPIController GetKPIController()
        {
            userControlSession = FakeUserControlSession();
            baseService = Substitute.For<IBaseService>();
            var controller = new JobLevelStandardKPIController(baseService)
            {
                UserControlSession = userControlSession
            };

            return controller;
        }
        #endregion Helpers
    }
}
