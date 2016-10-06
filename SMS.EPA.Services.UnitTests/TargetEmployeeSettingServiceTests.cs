namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Repositories;
    using SMS.EPA.Criteria.TargetEmployeeSetting;
    using SMS.EPA.Model;
    using SMS.EPA.Repositories.Interfaces;
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

        #region AssignTargetTemplateToEmployee

        [Test]
        public void AssignTargetTemplateToEmployee_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeTargetTemplateSetting();

            // Act
            var actual = targetEmployeeSettingService.AssignTargetTemplateToEmployee(Arg.Any<AssignTargetTemplateToEmployeeCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void AssignTargetTemplateToEmployee_ValidArg_ExpectedToExecuteUpdate()
        {
            // Arrange
            var criteria = new AssignTargetTemplateToEmployeeCriteria()
            {
                TargetTemplateId = 1
            };
            ArrangeTargetTemplateSetting();

            // Act
            targetEmployeeSettingService.AssignTargetTemplateToEmployee(criteria);

            // Assert
            targetEmployeeSettingService.Repository.Received().Update(Arg.Is<AssignTargetTemplateToEmployeeCriteria>(x => x.IsValid()));
        }

        #endregion AssignTargetTemplateToEmployee

        #region AssignTargetTemplateToDepartment

        [Test]
        public void AssignTargetTemplateToDepartment_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeTargetTemplateSetting();

            // Act
            var actual = targetEmployeeSettingService.AssignTargetTemplateToDepartment(Arg.Any<AssignTargetDepartmentSettingCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void AssignTargetTemplateToDepartment_ValidArg_ExpectedToExecuteUpdate()
        {
            // Arrange
            var criteria = new AssignTargetDepartmentSettingCriteria()
            {
                TargetTemplateId = 1
            };
            ArrangeTargetTemplateSetting();

            // Act
            targetEmployeeSettingService.AssignTargetTemplateToDepartment(criteria);

            // Assert
            targetEmployeeSettingService.Repository.Received().Update(Arg.Is<AssignTargetDepartmentSettingCriteria>(x => x.IsValid()));
        }

        #endregion AssignTargetTemplateToDepartment

        #region DeleteTargetEmployeeSetting

        [Test]
        public void DeleteTargetEmployeeSetting_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeTargetTemplateSetting();

            // Act
            var actual = targetEmployeeSettingService.DeleteTargetEmployeeSetting(Arg.Any<DeleteTargetEmployeeSettingCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void DeleteTargetEmployeeSetting_ValidArg_ExpectedToExecuteUpdate()
        {
            // Arrange
            var criteria = new DeleteTargetEmployeeSettingCriteria()
            {
                EmployeeId = 1,
                TargetTemplateId = 1
            };
            ArrangeTargetTemplateSetting();

            // Act
            targetEmployeeSettingService.DeleteTargetEmployeeSetting(criteria);

            // Assert
            targetEmployeeSettingService.Repository.Received().Delete(Arg.Is<DeleteTargetEmployeeSettingCriteria>(x => x.IsValid()));
        }

        #endregion DeleteTargetEmployeeSetting

        #region DeleteTargetDepartmentSetting

        [Test]
        public void DeleteTargetDepartmentSetting_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeTargetTemplateSetting();

            // Act
            var actual = targetEmployeeSettingService.DeleteTargetDepartmentSetting(Arg.Any<DeleteTargetDepartmentSettingCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void DeleteTargetDepartmentSetting_ValidArg_ExpectedToExecuteUpdate()
        {
            // Arrange
            var criteria = new DeleteTargetDepartmentSettingCriteria()
            {
                DepartmentId = 1,
                TargetTemplateId = 1
            };
            ArrangeTargetTemplateSetting();

            // Act
            targetEmployeeSettingService.DeleteTargetDepartmentSetting(criteria);

            // Assert
            targetEmployeeSettingService.Repository.Received().Delete(criteria);
        }

        #endregion DeleteTargetDepartmentSetting

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

        private void ArrangeTargetTemplateSetting(CommandResult result = null)
        {
            var baseRepository = FakeBaseRepository();
            targetEmployeeSettingService = GetTargetEmployeeSettingService(baseRepository);
            targetEmployeeSettingService.Repository.Insert(Arg.Any<ICriteria>()).Returns(result ?? new CommandResult());
            targetEmployeeSettingService.Repository.Update(Arg.Any<ICriteria>()).Returns(result ?? new CommandResult());
            targetEmployeeSettingService.Repository.Delete(Arg.Any<ICriteria>()).Returns(result ?? new CommandResult());
        }
    }
}