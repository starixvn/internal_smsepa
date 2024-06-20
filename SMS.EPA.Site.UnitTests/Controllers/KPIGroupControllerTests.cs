namespace SMS.EPA.Site.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using GO.UserControl.Client.Core.Session;
    using Model;
    using NSubstitute;
    using NUnit.Framework;
    using Site.Controllers.API;
    using SMS.EPA.Core.Constants;

    [TestFixture]
    public class KPIGroupControllerTests
    {
        private IBaseService baseService;
        private IUserControlSession userControlSession;

        [Test]
        public void GetKPIGroup_WhenCalled_ReturnListOfKPIGroups()
        {
            // Arrange
            IEnumerable<KPIGroupModel> result = GetListOfKPIGroupForTest();
            var controller = GetKPIGroupController();
            baseService.GetData<KPIGroupModel>(Arg.Any<GetKPIGroupCriteria>()).Returns(result);

            // Act
            var actual = controller.GetKPIGroup(Arg.Any<bool?>());

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(result, actual);
        }

        [Test]
        public void GetKPIGroupById_WhenCalled_ReturnExpectedKPIGroup()
        {
            // Arrange
            int kpiGroupId = 1;
            KPIGroupModel result = GetListOfKPIGroupForTest().First();
            var controller = GetKPIGroupController();
            baseService.GetDataEntity<KPIGroupModel>(Arg.Any<GetKPIGroupByIdCriteria>()).Returns(result);

            // Act
            var actual = controller.GetKPIGroupById(kpiGroupId);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(result, actual);
        }

        [Test]
        public void CreateKPIGroup_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetKPIGroupController();
            baseService.ExecuteNonQuery(Arg.Any<CreateKPIGroupCriteria>()).Returns(expected);

            // Act
            var actual = controller.CreateKPIGroup(Arg.Any<CreateKPIGroupCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UpdateKPIGroup_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetKPIGroupController();
            baseService.ExecuteNonQuery(Arg.Any<UpdateKPIGroupCriteria>()).Returns(expected);

            // Act
            var actual = controller.UpdateKPIGroup(Arg.Any<UpdateKPIGroupCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UpdateStatusKPIGroup_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetKPIGroupController();
            baseService.ExecuteNonQuery(Arg.Any<UpdateStatusKPIGroupCriteria>()).Returns(expected);

            // Act
            var actual = controller.UpdateStatusKPIGroup(Arg.Any<UpdateStatusKPIGroupCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DeleteKPIGroup_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var controller = GetKPIGroupController();
            baseService.ExecuteNonQuery(Arg.Any<DeleteKPIGroupCriteria>()).Returns(expected);

            // Act
            var actual = controller.DeleteKPIGroup(Arg.Any<int>());

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

        private KPIGroupController GetKPIGroupController()
        {
            userControlSession = FakeUserControlSession();
            baseService = Substitute.For<IBaseService>();
            var controller = new KPIGroupController(baseService) { UserControlSession = userControlSession };

            return controller;
        }

        private IEnumerable<KPIGroupModel> GetListOfKPIGroupForTest()
        {
            var kpiGroups = new List<KPIGroupModel>
            {
                new KPIGroupModel
                {
                    KPIGroupId = 1,
                    Name = "group test 1"
                },
                new KPIGroupModel
                {
                    KPIGroupId = 2,
                    Name = "group test 2"
                }
            };

            return kpiGroups;
        }

        #endregion Helpers
    }
}