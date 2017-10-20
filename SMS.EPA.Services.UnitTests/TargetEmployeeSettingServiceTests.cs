namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Repositories;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Repositories;
    using SMS.EPA.Services.Implement;

    [TestFixture]
    public class TargetEmployeeSettingServiceTests
    {
        private ITargetEmployeeSettingRepository targetEmployeeSettingRepository;
        private TargetEmployeeSettingService targetEmployeeSettingService;

        #region GetTargetEmployeeSetting

        [Test]
        public void GetTargetEmployeeSetting_AnyArg_ReturnInstanceOfListOfTargetEmployeeSetting()
        {
            // Arrange
            targetEmployeeSettingService = Substitute.For<TargetEmployeeSettingService>();

            // Act
            var actual = targetEmployeeSettingService.GetTargetEmployeeSetting(Arg.Any<GetAllTargetEmployeeSettingCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<TargetEmployeeSetting>), actual);
        }

        [Test]
        public void GetTargetEmployeeSetting_VaidArg_ReturnExpectedTargetEmployees()
        {
            // Arrange
            var criteria = new GetAllTargetEmployeeSettingCriteria();
            var expectedTargetEmployees = new List<TargetEmployeeSetting>();
            expectedTargetEmployees.Add(new TargetEmployeeSetting());
            ArrangeGetTargetEmployeeSetting(expectedTargetEmployees);

            // Act
            var actual = targetEmployeeSettingService.GetTargetEmployeeSetting(criteria);

            // Assert
            Assert.AreEqual(expectedTargetEmployees, actual);
        }

        [Test]
        public void GetTargetEmployeeSetting_InVaidArg_ReturnEmptyListOfTargetEmployee()
        {
            // Arrange
            var expectedTargetEmployees = new List<TargetEmployeeSetting>();
            expectedTargetEmployees.Add(new TargetEmployeeSetting());
            ArrangeGetTargetEmployeeSetting(expectedTargetEmployees);

            // Act
            var actual = targetEmployeeSettingService.GetTargetEmployeeSetting(null);

            // Assert
            Assert.AreNotEqual(expectedTargetEmployees, actual);
            Assert.AreEqual(actual.Count, 0);
        }

        #endregion GetTargetEmployeeSetting

        #region GetTargetDepartmentSetting

        [Test]
        public void GetTargetDepartmentSetting_AnyArg_ReturnInstanceOfListOfTargetDepartmentSetting()
        {
            // Arrange
            targetEmployeeSettingService = Substitute.For<TargetEmployeeSettingService>();

            // Act
            var actual = targetEmployeeSettingService.GetTargetDepartmentSetting(Arg.Any<GetTargetDepartmentSettingCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<TargetDepartmentSetting>), actual);
        }

        [Test]
        public void GetTargetDepartmentSetting_VaidArg_ReturnExpectedTargetDepartments()
        {
            // Arrange
            var criteria = new GetTargetDepartmentSettingCriteria();
            var expectedTargetDepartments = new List<TargetDepartmentSetting>();
            expectedTargetDepartments.Add(new TargetDepartmentSetting());
            ArrangeGetTargetDepartmentSetting(expectedTargetDepartments);

            // Act
            var actual = targetEmployeeSettingService.GetTargetDepartmentSetting(criteria);

            // Assert
            Assert.AreEqual(expectedTargetDepartments, actual);
        }

        [Test]
        public void GetTargetEmployeeSetting_InVaidArg_ReturnEmptyListOfTargetDepartment()
        {
            // Arrange
            var expectedTargetEmployees = new List<TargetDepartmentSetting>();
            expectedTargetEmployees.Add(new TargetDepartmentSetting());
            ArrangeGetTargetDepartmentSetting(expectedTargetEmployees);

            // Act
            var actual = targetEmployeeSettingService.GetTargetEmployeeSetting(null);

            // Assert
            Assert.AreNotEqual(expectedTargetEmployees, actual);
            Assert.AreEqual(actual.Count, 0);
        }

        #endregion GetTargetDepartmentSetting

        private IBaseRepository FakeBaseRepository()
        {
            var repository = Substitute.For<IBaseRepository>();

            return repository;
        }

        private TargetEmployeeSettingService GetTargetEmployeeSettingService(IBaseRepository baseRepository = null)
        {
            var service = new TargetEmployeeSettingService();
            service.Repository = baseRepository;

            return service;
        }

        private void ArrangeGetTargetEmployeeSetting(IEnumerable<TargetEmployeeSetting> targetEmployees = null)
        {
            targetEmployeeSettingRepository = Substitute.For<ITargetEmployeeSettingRepository>();

            targetEmployeeSettingRepository.GetTargetTemplateEmployee(Arg.Any<GetAllTargetEmployeeSettingCriteria>())
                .Returns(targetEmployees ?? new List<TargetEmployeeSetting>());
            targetEmployeeSettingService = new TargetEmployeeSettingService(targetEmployeeSettingRepository);
        }

        private void ArrangeGetTargetDepartmentSetting(IEnumerable<TargetDepartmentSetting> targetDepartments = null)
        {
            targetEmployeeSettingRepository = Substitute.For<ITargetEmployeeSettingRepository>();

            targetEmployeeSettingRepository.GetTargetTemplateDepartment(Arg.Any<GetTargetDepartmentSettingCriteria>())
                .Returns(targetDepartments ?? new List<TargetDepartmentSetting>());
            targetEmployeeSettingService = new TargetEmployeeSettingService(targetEmployeeSettingRepository);
        }
    }
}