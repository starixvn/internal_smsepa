namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using Core.Enumerations;
    using Core.Model;
    using Core.Security;
    using Core.Services;
    using Criteria;
    using Implement;
    using Model.Enitites;
    using NSubstitute;
    using NUnit.Framework;
    using Repositories;

    [TestFixture]
    public class EmployeeServiceTests
    {
        private IEmployeeRepository _employeeRepository;
        private IBaseService baseService;
        private EmployeeService _employeeService;

        #region GetAllEmployee

        [Test]
        public void GetAllEmployee_AnyArg_ReturnInstanceOfListOfEmployeeModel()
        {
            // Arrange
            ArrangeGetAllEmployee();

            // Act
            var actual = _employeeService.GetAllEmployee(Arg.Any<GetAllEmployeeCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<Employee>), actual);
        }

        [Test]
        public void GetAllEmployee_ValidArg_ExpectedToExecuteGetAllEmployee()
        {
            // Arrange
            var criteria = new GetAllEmployeeCriteria();
            ArrangeGetAllEmployee();

            // Act
            _employeeService.GetAllEmployee(criteria);

            // Assert
            _employeeRepository.Received().GetAllEmployee(Arg.Is<GetAllEmployeeCriteria>(x => x.IsValid()));
        }

        #endregion GetAllEmployee

        #region GetEmployeeById

        [Test]
        public void GetEmployeeId_ValidArg_ReturnExpectedEmployee()
        {
            // Arrange
            var employeeId = 1;
            var expectedEmployee = new Employee()
            {
                EmployeeId = employeeId
            };
            ArrangeGetEmployeeById(expectedEmployee);

            // Act
            var actual = _employeeService.GetEmployeeById(employeeId);

            // Assert
            Assert.AreEqual(expectedEmployee, actual);
        }

        [Test]
        public void GetEmployeeId_InValidArg_ReturnDefaultEmployee()
        {
            // Arrange
            var employeeId = -1;
            var expectedEmployee = new Employee()
            {
                EmployeeId = employeeId
            };
            ArrangeGetEmployeeById(expectedEmployee);

            // Act
            var actual = _employeeService.GetEmployeeById(employeeId);

            // Assert
            Assert.AreNotEqual(expectedEmployee, actual);
        }

        #endregion GetEmployeeById

        #region GetEmployeeLogin

        [Test]
        public void GetEmployeeLogin_AnyArg_ReturnInstanceOfUserSession()
        {
            // Arrange
            ArrangeGetEmployeeLogin();

            // Act
            var actual = _employeeService.GetEmployeeLogin(Arg.Any<GetEmployeeLoginCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(UserSession), actual);
        }

        [Test]
        public void GetEmployeeLogin_ValidArg_ExpectedToExecuteGetEmployeeLogin()
        {
            // Arrange
            var criteria = new GetEmployeeLoginCriteria
            {
                Code = "00329"
            };
            ArrangeGetEmployeeLogin();

            // Act
            _employeeService.GetEmployeeLogin(criteria);

            // Assert
            _employeeRepository.Received().GetEmployeeLogin(Arg.Is<GetEmployeeLoginCriteria>(x => x.IsValid()));
        }

        #endregion GetEmployeeLogin

        #region CreateSpecialEmployee

        [Test]
        public void CreateSpecialEmployee_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeCreateSpecialEmployee();

            // Act
            var actual = _employeeService.CreateSpecialEmployee(new CreateSpecialEmployeeCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void CreateSpecialEmployee_ValidArg_ExpectedToEncryptPassword()
        {
            // Arrange
            var criteria = new CreateSpecialEmployeeCriteria
            {
                NickName = "test",
                FullName = "test",
                Username = "test",
                Password = "123456",
                CreatedBy = 1
            };
            var plainText = "123456";
            ArrangeCreateSpecialEmployee();

            // Act
            _employeeService.CreateSpecialEmployee(criteria);

            // Assert
            Assert.AreNotEqual(plainText, criteria.Password);
        }

        [Test]
        public void CreateSpecialEmployee_ValidArg_ExpectedToExecuteInsert()
        {
            // Arrange
            var criteria = new CreateSpecialEmployeeCriteria
            {
                NickName = "test",
                FullName = "test",
                Username = "test",
                Password = "123456",
                CreatedBy = 1
            };
            ArrangeCreateSpecialEmployee();

            // Act
            _employeeService.CreateSpecialEmployee(criteria);

            // Assert
            baseService.Received().ExecuteNonQuery(Arg.Is<CreateSpecialEmployeeCriteria>(x => x.IsValid()));
        }

        #endregion CreateSpecialEmployee

        #region ChangePassword

        [Test]
        public void ChangePassword_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeChangePassword();

            // Act
            var actual = _employeeService.ChangePassword(new ChangePasswordCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void ChangePassword_ValidArg_ExpectedToEncryptPassword()
        {
            // Arrange
            var oldPassword = "123456";
            var newPassword = "1234567";
            var criteria = new ChangePasswordCriteria
            {
                EmployeeId = 1,
                NewPassword = newPassword,
                OldPassword = oldPassword,
                ConfirmPassword = newPassword,
                ModifiedBy = 1
            };

            ArrangeChangePassword();

            // Act
            _employeeService.ChangePassword(criteria);

            // Assert
            Assert.AreNotEqual(newPassword, criteria.NewPassword);
            Assert.IsTrue(Encryption.CompareStringToHash(HashProvider.Sha1, newPassword, criteria.NewPassword));
        }

        [Test]
        public void ChangePassword_WrongOldPassword_ReturnChangeFail()
        {
            // Arrange
            var oldPassword = "asasas";
            var newPassword = "1234567";
            var criteria = new ChangePasswordCriteria
            {
                EmployeeId = 1,
                NewPassword = newPassword,
                OldPassword = oldPassword,
                ConfirmPassword = newPassword,
                ModifiedBy = 1
            };

            ArrangeChangePassword();

            // Act
            CommandResult result = _employeeService.ChangePassword(criteria);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
        }

        [Test]
        public void ChangePassword_InValidArg_ReturnChangeFail()
        {
            // Arrange
            var oldPassword = "123456";
            var newPassword = "1234567";
            var confirmPassword = "123333";
            var criteria = new ChangePasswordCriteria
            {
                EmployeeId = 1,
                NewPassword = newPassword,
                OldPassword = oldPassword,
                ConfirmPassword = confirmPassword,
                ModifiedBy = 1
            };

            ArrangeChangePassword();

            // Act
            CommandResult result = _employeeService.ChangePassword(criteria);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
        }

        #endregion ChangePassword

        #region ResetPassword

        [Test]
        public void ResetPassword_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeResetPassword();

            // Act
            var actual = _employeeService.ResetPassword(new ResetPasswordCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void ResetPassword_ValidArg_ExpectedToEncryptPassword()
        {
            // Arrange
            var newPassword = "1234567";
            var criteria = new ResetPasswordCriteria
            {
                EmployeeId = 1,
                NewPassword = newPassword,
                ModifiedBy = 1
            };

            ArrangeResetPassword();

            // Act
            _employeeService.ResetPassword(criteria);

            // Assert
            Assert.AreNotEqual(newPassword, criteria.NewPassword);
            Assert.IsTrue(Encryption.CompareStringToHash(HashProvider.Sha1, newPassword, criteria.NewPassword));
        }

        #endregion ResetPassword

        #region Helpers

        private IBaseService FakeBaseService()
        {
            var repository = Substitute.For<IBaseService>();

            return repository;
        }

        private IEmployeeRepository FakeEmployeeRepository()
        {
            var repository = Substitute.For<IEmployeeRepository>();

            return repository;
        }

        private EmployeeService GetEmployeeService(
            IEmployeeRepository employeeRepository = null,
            IBaseService baseService = null)
        {
            var service = new EmployeeService(employeeRepository, baseService);
            return service;
        }

        private void ArrangeGetEmployeeLogin(UserSession result = null)
        {
            _employeeRepository = FakeEmployeeRepository();
            _employeeRepository.GetEmployeeLogin(Arg.Any<GetEmployeeLoginCriteria>()).Returns(result ?? new UserSession());
            _employeeService = GetEmployeeService(_employeeRepository);
        }

        private void ArrangeGetAllEmployee(IEnumerable<Employee> result = null)
        {
            _employeeRepository = FakeEmployeeRepository();
            _employeeRepository.GetAllEmployee(Arg.Any<GetAllEmployeeCriteria>()).Returns(result ?? new List<Employee>());
            _employeeService = GetEmployeeService(_employeeRepository);
        }

        private void ArrangeGetEmployeeById(Employee result = null)
        {
            _employeeRepository = FakeEmployeeRepository();
            _employeeService = GetEmployeeService(_employeeRepository);
            _employeeRepository.GetEmployeeById(Arg.Any<GetEmployeeByIdCriteria>()).Returns(result ?? new Employee());
        }

        private void ArrangeCreateSpecialEmployee(CommandResult result = null)
        {
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<CreateSpecialEmployeeCriteria>()).Returns(result ?? new CommandResult());
            _employeeService = GetEmployeeService(null, baseService);
        }

        private void ArrangeChangePassword(CommandResult result = null)
        {
            baseService = FakeBaseService();
            baseService.GetDataEntity<string>(Arg.Any<GetPasswordByEmpIdCriteria>()).Returns(Encryption.ComputeHash(HashProvider.Sha1, "123456"));
            baseService.ExecuteNonQuery(Arg.Any<ChangePasswordCriteria>()).Returns(result ?? new CommandResult());
            _employeeService = GetEmployeeService(null, baseService);
        }

        private void ArrangeResetPassword(CommandResult result = null)
        {
            baseService = FakeBaseService();
            baseService.ExecuteNonQuery(Arg.Any<ResetPasswordCriteria>()).Returns(result ?? new CommandResult());
            _employeeService = GetEmployeeService(null, baseService);
        }

        #endregion Helpers
    }
}