namespace SMS.EPA.Site.UnitTests.Controllers
{
    using System.Collections.Generic;
    using GO.UserControl.Client.Core.Session;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.JobLevelStandard.Template;
    using SMS.EPA.Model.JobLevelStandard.Template;
    using SMS.EPA.Site.Controllers.API.JobLevelStandard;

    [TestFixture]
    public class JobLevelStandardTemplateControllerTests
    {
        private IBaseService baseService;
        private IUserControlSession userControlSession;

        #region GetTemplate

        [Test]
        public void GetTemplates_WhenCalled_ReturnExpectedTemplates()
        {
            // Arrange
            var templates = new List<JobLevelStandardTemplate>();
            var controller = GetController();
            baseService.GetData<JobLevelStandardTemplate>(Arg.Any<GetJobLevelStandardTemplateCriteria>()).Returns(templates);

            // Act
            var actual = controller.GetTemplate(Arg.Any<GetJobLevelStandardTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<JobLevelStandardTemplate>), actual);
            Assert.AreEqual(templates, actual);
        }

        #endregion GetTemplate

        #region GetTemplateById

        [Test]
        public void GetTemplateById_WhenCalled_ReturnExpectedTemplates()
        {
            // Arrange
            var template = new JobLevelStandardTemplate();
            var controller = GetController();
            baseService.GetDataEntity<JobLevelStandardTemplate>(Arg.Any<GetJobLevelStandardTemplateByIdCriteria>()).Returns(template);

            // Act
            var actual = controller.GetById(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<JobLevelStandardTemplate>), actual);
            Assert.AreEqual(template, actual);
        }

        #endregion GetTemplateById

        #region GetKPIList

        [Test]
        public void GetKPIList_WhenCalled_ReturnExpectedKPIList()
        {
            // Arrange
            var templates = new List<JobLevelStandardTemplateKPI>();
            var controller = GetController();
            baseService.GetData<JobLevelStandardTemplateKPI>(Arg.Any<GetKPIListCriteria>()).Returns(templates);

            // Act
            var actual = controller.GetKPI(Arg.Any<GetKPIListCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<JobLevelStandardTemplateKPI>), actual);
            Assert.AreEqual(templates, actual);
        }

        #endregion GetKPIList

        #region CreateTemplate

        [Test]
        public void CreateTemplate_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetController();
            baseService.ExecuteNonQuery(Arg.Any<CreateJobLevelStandardTemplateCriteria>()).Returns(expected);

            // Act
            var actual = controller.Create(Arg.Any<CreateJobLevelStandardTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion CreateTemplate

        #region UpdateStatusTemplate

        [Test]
        public void UpdateStatusTemplate_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetController();
            baseService.ExecuteNonQuery(Arg.Any<UpdateStatusJobLevelStandardTemplateCriteria>()).Returns(expected);

            // Act
            var actual = controller.UpdateStatus(Arg.Any<UpdateStatusJobLevelStandardTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion UpdateStatusTemplate

        #region UpdateTemplate

        [Test]
        public void UpdateTemplate_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetController();
            baseService.ExecuteNonQuery(Arg.Any<UpdateJobLevelStandardTemplateCriteria>()).Returns(expected);

            // Act
            var actual = controller.Update(Arg.Any<UpdateJobLevelStandardTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion UpdateTemplate

        #region UpdateTemplateKPI

        [Test]
        public void UpdateTemplateKPI_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetController();
            baseService.ExecuteNonQuery(Arg.Any<UpdateJobLevelStandardTemplateKPICriteria>()).Returns(expected);

            // Act
            var actual = controller.UpdateTemplateKPI(Arg.Any<UpdateJobLevelStandardTemplateKPICriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion UpdateTemplateKPI

        #region DeleteTemplate

        [Test]
        public void DeleteTemplate_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetController();
            baseService.ExecuteNonQuery(Arg.Any<DeleteJobLevelStandardTemplateCriteria>()).Returns(expected);

            // Act
            var actual = controller.Delete(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion DeleteTemplate

        #region Helpers
        private static IUserControlSession FakeUserControlSession()
        {
            var session = Substitute.For<IUserControlSession>();
            session.Set<int>(Constants.CurrentEmployeeIdKey, 1);

            return session;
        }
        private JobLevelStandardTemplateController GetController()
        {
            userControlSession = FakeUserControlSession();
            baseService = Substitute.For<IBaseService>();
            var controller = new JobLevelStandardTemplateController(baseService)
            {
                UserControlSession = userControlSession
            };

            return controller;
        }
        #endregion Helpers
    }
}
