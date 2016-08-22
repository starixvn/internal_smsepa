namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Repositories;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Implement;

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

        #region GetEmployeeById

        [Test]
        public void GetEmployeeById_AnyArg_ReturnInstanceOfEmployeeModel()
        {
            // Arrange
            ArrangeGetEmployeeLogin();

            // Act
            var actual = employeeService.GetEmployeeById(Arg.Any<GetEmployeeByIdCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(EmployeeModel), actual);
        }

        [Test]
        public void GetEmployeeById_ValidArg_ExpectedToExecuteGetEmployeeById()
        {
            // Arrange
            var criteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = 1
            };
            ArrangeGetEmployeeLogin();

            // Act
            employeeService.GetEmployeeById(criteria);

            // Assert
            employeeRepository.Received().GetEmployeeById(Arg.Is<GetEmployeeByIdCriteria>(x => x.IsValid()));
        }

        #endregion GetEmployeeById

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

        private void ArrangeGetAllGroup(IEnumerable<Group> result = null)
        {
            employeeService = Substitute.For<EmployeeService>();
            employeeService.When(x => x.GetData<Group>(Arg.Any<GetAllGroupCriteria>())).DoNotCallBase();
            employeeService.GetData<Group>(Arg.Any<GetAllGroupCriteria>()).Returns(result ?? new List<Group>());
        }

        #endregion Helpers
    }
}