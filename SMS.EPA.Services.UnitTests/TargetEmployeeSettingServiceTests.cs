namespace SMS.EPA.Services.UnitTests
{
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Criteria;
    using SMS.EPA.Criteria.TargetEmployeeSetting;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.TargetEmployeeSetting;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;
    using System.Collections.Generic;

    [TestFixture]
    public class TargetEmployeeSettingServiceTests
    {
        private ITargetEmployeeSettingRepository targetEmployeeSettingRepository;
        private ITargetEmployeeSettingService targetEmployeeSettingService;

        #region GetTargetEmployeeSetting
        [Test]
        public void GetTargetEmployeeSetting_AnyArg_ReturnInstanceOfListOfTargetTypeModel()
        {
            // Arrange
            targetEmployeeSettingService = Substitute.For<ITargetEmployeeSettingService>();

            // Act
            var actual = targetEmployeeSettingService.GetTargetEmployeeSetting(Arg.Any<GetAllTargetEmployeeSettingCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<TargetEmployeeModel>), actual);
        }

        [Test]
        public void GetTargetEmployeeSetting_VaidArg_ReturnExpectedTargetTypes()
        {
            // Arrange
            var criteria = new GetAllTargetEmployeeSettingCriteria();
            var expectedTargetEmployees = new List<TargetEmployeeModel>();
            expectedTargetEmployees.Add(new TargetEmployeeModel());
            ArrangeGetTargetEmployeeSetting(expectedTargetEmployees);

            // Act
            var actual = targetEmployeeSettingService.GetTargetEmployeeSetting(criteria);

            // Assert
            Assert.AreEqual(expectedTargetEmployees, actual);
        }

        #endregion

        #region GetAllGroup
        [Test]
        public void GetAllGroup_AnyArg_ReturnInstanceOfListOfGroup()
        {
            ////// Arrange
            ////commonService = Substitute.For<ICommonService>();

            ////// Act
            ////var actual = commonService.GetAllGroup(Arg.Any<GetAllGroupCriteria>());

            ////// Assert
            ////Assert.IsInstanceOf(typeof(IEnumerable<Group>), actual);
        }

        [Test]
        public void GetAllGroup_VaidArg_ReturnExpectedGroups()
        {
            // Arrange
            var criteria = new GetAllGroupCriteria();
            var expectedGroups = new List<Group>();
            expectedGroups.Add(new Group());
            var commonService = ArrangeGetAllGroup(expectedGroups);

            // Act
            var actual = commonService.GetAllGroup(criteria);

            // Assert
            Assert.AreEqual(expectedGroups, actual);
        }

        #endregion

        private void ArrangeGetTargetEmployeeSetting(IEnumerable<TargetEmployeeModel> targetEmployees = null)
        {
            targetEmployeeSettingRepository = Substitute.For<ITargetEmployeeSettingRepository>();

            //// targetEmployeeSettingRepository.GetTargetEmployeeSetting(Arg.Any<GetAllTargetEmployeeSettingCriteria>()).Returns(targetEmployees);
        }

        private CommonService ArrangeGetAllGroup(IEnumerable<Group> groups = null)
        {
            var commonService = Substitute.For<CommonService>();
            commonService.GetData<Group>(Arg.Any<GetAllGroupCriteria>()).Returns(groups ?? new List<Group>());
            return commonService;
        }
    }
}
