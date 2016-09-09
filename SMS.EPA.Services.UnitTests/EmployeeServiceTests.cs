namespace SMS.EPA.Services.UnitTests
{
    using Fanex.Data.Repository;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Repositories;
    using SMS.EPA.Core.Security;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Implement;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class EmployeeServiceTests
    {
        private IEmployeeRepository employeeRepository;
        private IBaseRepository baseRepository;
        private EmployeeService employeeService;

        #region GetAllEmployee

        [Test]
        public void GetAllEmployee_AnyArg_ReturnInstanceOfListOfEmployeeModel()
        {
            // Arrange
            ArrangeGetAllEmployee();

            // Act
            var actual = employeeService.GetAllEmployee(Arg.Any<GetAllEmployeeCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<EmployeeModel>), actual);
        }

        [Test]
        public void GetAllEmployee_ValidArg_ExpectedToExecuteGetAllEmployee()
        {
            // Arrange
            var criteria = new GetAllEmployeeCriteria();
            ArrangeGetAllEmployee();

            // Act
            employeeService.GetAllEmployee(criteria);

            // Assert
            employeeRepository.Received().GetAllEmployee(Arg.Is<GetAllEmployeeCriteria>(x => x.IsValid()));
        }

        #endregion GetAllEmployee

        #region GetEmployeeById

        [Test]
        public void GetEmployeeId_ValidArg_ReturnExpectedEmployee()
        {
            // Arrange
            var employeeId = 1;
            var expectedEmployee = new EmployeeModel()
            {
                EmployeeId = employeeId
            };
            ArrangeGetEmployeeById(expectedEmployee);

            // Act
            var actual = employeeService.GetEmployeeById(employeeId);

            // Assert
            Assert.AreEqual(expectedEmployee, actual);
        }

        [Test]
        public void GetEmployeeId_InValidArg_ReturnDefaultEmployee()
        {
            // Arrange
            var employeeId = -1;
            var expectedEmployee = new EmployeeModel()
            {
                EmployeeId = employeeId
            };
            ArrangeGetEmployeeById(expectedEmployee);

            // Act
            var actual = employeeService.GetEmployeeById(employeeId);

            // Assert
            Assert.AreNotEqual(expectedEmployee, actual);
        }

        ////[Test]
        ////public void GetEmployeeById_AnyArg_ReturnInstanceOfEmployeeModel()
        ////{
        ////    // Arrange
        ////    var employeeId = 1;
        ////    ArrangeGetEmployeeById();

        ////    // Act
        ////    var actual = employeeService.GetEmployeeById(employeeId);

        ////    // Assert
        ////    Assert.IsInstanceOf(typeof(EmployeeModel), actual);
        ////}

        ////[Test]
        ////public void GetEmployeeById_ValidArg_ExpectedToExecuteGetEmployeeById()
        ////{
        ////    // Arrange
        ////    int employeeId = 1;
        ////    ArrangeGetEmployeeById();

        ////    // Act
        ////    employeeService.GetEmployeeById(employeeId);

        ////    // Assert
        ////    employeeRepository.Received().GetEmployeeById(Arg.Is<GetEmployeeByIdCriteria>(y => y.IsValid()));
        ////}

        #endregion GetEmployeeById

        #region GetEmployeeLogin

        [Test]
        public void GetEmployeeLogin_AnyArg_ReturnInstanceOfUserSession()
        {
            // Arrange
            ArrangeGetEmployeeLogin();

            // Act
            var actual = employeeService.GetEmployeeLogin(Arg.Any<GetEmployeeLoginCriteria>());

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
            employeeService.GetEmployeeLogin(criteria);

            // Assert
            employeeRepository.Received().GetEmployeeLogin(Arg.Is<GetEmployeeLoginCriteria>(x => x.IsValid()));
        }

        #endregion GetEmployeeLogin

        #region CreateSpecialEmployee

        [Test]
        public void CreateSpecialEmployee_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeCreateSpecialEmployee();

            // Act
            var actual = employeeService.CreateSpecialEmployee(new CreateSpecialEmployeeCriteria());

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
            employeeService.CreateSpecialEmployee(criteria);

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
            employeeService.CreateSpecialEmployee(criteria);

            // Assert
            employeeService.Repository.Received().Insert(Arg.Is<CreateSpecialEmployeeCriteria>(x => x.IsValid()));
        }

        #endregion CreateSpecialEmployee

        #region UpdateSpecialEmployee

        [Test]
        public void UpdateSpecialEmployee_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeUpdateSpecialEmployee();

            // Act
            var actual = employeeService.UpdateSpecialEmployee(new UpdateSpecialEmployeeCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateSpecialEmployee_ValidArg_ExpectedToExecuteUpdate()
        {
            // Arrange
            var criteria = new UpdateSpecialEmployeeCriteria
            {
                EmployeeId = 1,
                NickName = "test",
                FullName = "test",
                Username = "test",
                ModifiedBy = 1
            };
            ArrangeUpdateSpecialEmployee();

            // Act
            employeeService.UpdateSpecialEmployee(criteria);

            // Assert
            employeeService.Repository.Received().Update(Arg.Is<UpdateSpecialEmployeeCriteria>(x => x.IsValid()));
        }

        #endregion UpdateSpecialEmployee

        #region ChangePassword

        [Test]
        public void ChangePassword_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeChangePassword();

            // Act
            var actual = employeeService.ChangePassword(new ChangePasswordCriteria());

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
            employeeService.ChangePassword(criteria);

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
            CommandResult result = employeeService.ChangePassword(criteria);

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
            CommandResult result = employeeService.ChangePassword(criteria);

            // Assert
            Assert.IsFalse(result.IsSuccessful);
        }

        ////[Test]
        ////public void ChangePassword_ValidArg_ExpectedToExecuteUpdate()
        ////{
        ////    // Arrange
        ////    var oldPassword = "123456";
        ////    var newPassword = "1234567";
        ////    var criteria = new ChangePasswordCriteria
        ////    {
        ////        EmployeeId = 1,
        ////        NewPassword = newPassword,
        ////        OldPassword = oldPassword,
        ////        ConfirmPassword = newPassword,
        ////        ModifiedBy = 1
        ////    };
        ////    ArrangeChangePassword();

        ////    // Act
        ////    employeeService.ChangePassword(criteria);

        ////    // Assert
        ////    employeeService.Repository.Received().Update(Arg.Is<ChangePasswordCriteria>(x => x != null));
        ////}

        #endregion ChangePassword

        #region AssignRoleToEmployee

        [Test]
        public void AssignRoleToEmployee_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeAssignRoleToEmployee();

            // Act
            var actual = employeeService.AssignRoleToEmployee(new AssignRoleToEmployeeCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void AssignRoleToEmployee_ValidArg_ExpectedToExecuteUpdate()
        {
            // Arrange
            var criteria = new AssignRoleToEmployeeCriteria
            {
                EmployeeId = 1,
                RoleIds = "1,2",
                ModifiedBy = 1
            };
            ArrangeAssignRoleToEmployee();

            // Act
            employeeService.AssignRoleToEmployee(criteria);

            // Assert
            employeeService.Repository.Received().Update(Arg.Is<AssignRoleToEmployeeCriteria>(x => x.IsValid()));
        }

        #endregion AssignRoleToEmployee

        #region GetEmployeeByDepartmentId

        [Test]
        public void GetEmployeeByDepartmentId_AnyArg_ReturnInstanceOfListOfEmployeeModel()
        {
            // Arrange
            ArrangeGetEmployeeByDepartmentId();

            // Act
            var actual = employeeService.GetEmployeeByDepartmentId(Arg.Any<int?>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<EmployeeModel>), actual);
        }

        [Test]
        public void GetEmployeeByDepartmentId_ValidArg_ExpectedToExecuteGetData()
        {
            // Arrange
            int? departmentId = null;
            ArrangeGetEmployeeByDepartmentId();

            // Act
            employeeService.GetEmployeeByDepartmentId(departmentId);

            // Assert
            employeeService.Received().GetData<EmployeeModel>(Arg.Is<GetEmployeeByDepartmentIdCriteria>(x => x.IsValid()));
        }

        [Test]
        public void GetEmployeeByDepartmentId_ValidArg_ExpectedToOrderDataByNickName()
        {
            // Arrange
            int? departmentId = null;
            var result = GetListOfEmployeeModelForTest();
            ArrangeGetEmployeeByDepartmentId(result);

            // Act
            var actual = employeeService.GetEmployeeByDepartmentId(departmentId);

            // Assert
            Assert.IsTrue(actual.SequenceEqual(result.OrderBy(x => x.NickName)));
        }

        #endregion GetEmployeeByDepartmentId

        #region GetEmployeeByGroupId

        [Test]
        public void GetEmployeeByGroupId_AnyArg_ReturnInstanceOfListOfEmployeeModel()
        {
            // Arrange
            int groupId = 1;
            ArrangeGetEmployeeByGroupId();

            // Act
            var actual = employeeService.GetEmployeeByGroupId(groupId);

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<EmployeeModel>), actual);
        }

        [Test]
        public void GetEmployeeByGroupId_ValidArg_ExpectedToExecuteGetData()
        {
            // Arrange
            int groupId = 1;
            ArrangeGetEmployeeByGroupId();

            // Act
            employeeService.GetEmployeeByGroupId(groupId);

            // Assert
            employeeService.Received().GetData<EmployeeModel>(Arg.Is<GetEmployeeByGroupIdCriteria>(x => x.IsValid()));
        }

        #endregion GetEmployeeByGroupId

        #region InsertEmployeeIPAddress

        [Test]
        public void InsertEmployeeIPAddress_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeInsertEmployeeIPAddress();

            // Act
            var actual = employeeService.InsertEmployeeIPAddress(new InsertEmployeeIPAddressCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void InsertEmployeeIPAddress_ValidArg_ExpectedToExecuteInsert()
        {
            // Arrange
            var criteria = new InsertEmployeeIPAddressCriteria();
            ArrangeInsertEmployeeIPAddress();

            // Act
            employeeService.InsertEmployeeIPAddress(criteria);

            // Assert
            employeeService.Repository.Received().Insert(Arg.Is<InsertEmployeeIPAddressCriteria>(x => x.IsValid()));
        }

        #endregion InsertEmployeeIPAddress

        #region UpdateHasMonthlyEPA

        [Test]
        public void UpdateHasMonthlyEPA_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeUpdateHasMonthlyEPA();

            // Act
            var actual = employeeService.UpdateHasMonthlyEPA(new UpdateHasMonthlyEPACriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateHasMonthlyEPA_ValidArg_ExpectedToExecuteUpdate()
        {
            // Arrange
            var criteria = new UpdateHasMonthlyEPACriteria
            {
                EmployeeId = 1,
                ModifiedBy = 1
            };
            ArrangeUpdateHasMonthlyEPA();

            // Act
            employeeService.UpdateHasMonthlyEPA(criteria);

            // Assert
            employeeService.Repository.Received().Update(Arg.Is<UpdateHasMonthlyEPACriteria>(x => x.IsValid()));
        }

        #endregion UpdateHasMonthlyEPA

        #region Helpers

        private IBaseRepository FakeBaseRepository()
        {
            var repository = Substitute.For<IBaseRepository>();

            return repository;
        }

        private IEmployeeRepository FakeEmployeeRepository()
        {
            var repository = Substitute.For<IEmployeeRepository>();

            return repository;
        }

        private EmployeeService GetEmployeeService(
            IEmployeeRepository employeeRepository = null,
            IBaseRepository baseRepository = null)
        {
            var service = new EmployeeService(employeeRepository);
            service.Repository = baseRepository;
            return service;
        }

        private IEnumerable<EmployeeModel> GetListOfEmployeeModelForTest()
        {
            var employees = new List<EmployeeModel>
            {
                new EmployeeModel
                {
                    EmployeeId = 1,
                    NickName = "tim"
                },
                new EmployeeModel
                {
                    EmployeeId = 2,
                    NickName = "quinn"
                }
            };

            return employees;
        }

        private void ArrangeGetEmployeeLogin(UserSession result = null)
        {
            employeeRepository = FakeEmployeeRepository();
            employeeRepository.GetEmployeeLogin(Arg.Any<GetEmployeeLoginCriteria>()).Returns(result ?? new UserSession());
            employeeService = GetEmployeeService(employeeRepository);
        }

        private void ArrangeGetAllEmployee(IEnumerable<EmployeeModel> result = null)
        {
            employeeRepository = FakeEmployeeRepository();
            employeeRepository.GetAllEmployee(Arg.Any<GetAllEmployeeCriteria>()).Returns(result ?? new List<EmployeeModel>());
            employeeService = GetEmployeeService(employeeRepository);
        }

        private void ArrangeGetEmployeeById(EmployeeModel result = null)
        {
            employeeRepository = FakeEmployeeRepository();
            employeeService = GetEmployeeService(employeeRepository);
            employeeRepository.GetEmployeeById(Arg.Any<GetEmployeeByIdCriteria>()).Returns(result == null ? new EmployeeModel() : result);
        }

        private void ArrangeCreateSpecialEmployee(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            employeeService = GetEmployeeService(null, baseRepository);
            employeeService.Repository.Insert(Arg.Any<CreateSpecialEmployeeCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeUpdateSpecialEmployee(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            employeeService = GetEmployeeService(null, baseRepository);
            employeeService.Repository.Update(Arg.Any<UpdateSpecialEmployeeCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeChangePassword(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            employeeService = Substitute.For<EmployeeService>();
            employeeService.Repository = baseRepository;
            employeeService.GetDataEntity<string>(Arg.Any<GetPasswordByEmpIdCriteria>()).Returns(Encryption.ComputeHash(HashProvider.Sha1, "123456"));
            employeeService.Repository.Update(Arg.Any<ChangePasswordCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeAssignRoleToEmployee(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            employeeService = GetEmployeeService(null, baseRepository);
            employeeService.Repository.Update(Arg.Any<AssignRoleToEmployeeCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeGetEmployeeByDepartmentId(IEnumerable<EmployeeModel> result = null)
        {
            employeeService = Substitute.For<EmployeeService>();
            employeeService.When(x => x.GetData<EmployeeModel>(Arg.Any<GetEmployeeByDepartmentIdCriteria>())).DoNotCallBase();
            employeeService.GetData<EmployeeModel>(Arg.Any<GetEmployeeByDepartmentIdCriteria>()).Returns(result ?? new List<EmployeeModel>());
        }

        private void ArrangeGetEmployeeByGroupId(IEnumerable<EmployeeModel> result = null)
        {
            employeeService = Substitute.For<EmployeeService>();
            employeeService.When(x => x.GetData<EmployeeModel>(Arg.Any<GetEmployeeByGroupIdCriteria>())).DoNotCallBase();
            employeeService.GetData<EmployeeModel>(Arg.Any<GetEmployeeByGroupIdCriteria>()).Returns(result ?? new List<EmployeeModel>());
        }

        private void ArrangeInsertEmployeeIPAddress(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            employeeService = GetEmployeeService(null, baseRepository);
            employeeService.Repository.Insert(Arg.Any<InsertEmployeeIPAddressCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeUpdateHasMonthlyEPA(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            employeeService = GetEmployeeService(null, baseRepository);
            employeeService.Repository.Update(Arg.Any<UpdateHasMonthlyEPACriteria>()).Returns(result ?? new CommandResult());
        }

        #endregion Helpers
    }
}