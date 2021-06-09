namespace SMS.EPA.WindowsService.UnitTests.Service
{
    using AutoFixture;
    using Fanex.Data.Repository;
    using NSubstitute;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Model.API;
    using SMS.EPA.WindowsService.Criteria;
    using SMS.EPA.WindowsService.Model;
    using SMS.EPA.WindowsService.Services;
    using SMS.EPA.WindowsService.ThirdPartyAPI;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class EmployeeServiceTests
    {
        private readonly Fixture fixture = new Fixture();
        private IDynamicRepository dynamicRepository;
        private IEmployeeAPI employeeApi;

        [Fact]
        public void GetEmployees_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var criteria = fixture.Create<GetEmployeeCriteria>();
            var expected = fixture.Create<IEnumerable<Employee>>();
            var service = GetService();
            dynamicRepository.Fetch<Employee>(Arg.Do<GetEmployeeCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var employees = service.GetEmployees(criteria.FromDate);

            // Assert
            Assert.Equal(expected, employees);
        }

        [Fact]
        public async Task GetLastModifiedDateAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var token = fixture.Create<TokenAccess>();
            var service = GetService();
            var expected = fixture.Create<DateTime>();
            employeeApi.GetLastModifiedDateAsync(token).Returns(expected);

            // Act
            var actual = await service.GetLastModifiedDateAsync(token);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UpdateAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var token = fixture.Create<TokenAccess>();
            var employees = fixture.Create<IEnumerable<Employee>>();
            var expected = fixture.Create<CommandResult>();
            var service = GetService();
            employeeApi.Update(employees, token).Returns(expected);

            // Act
            var actual = service.UpdateAsync(employees, token);

            // Assert
            Assert.Equal(expected, actual.Result);
        }

        private EmployeeService GetService()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            employeeApi = Substitute.For<IEmployeeAPI>();
            return new EmployeeService(dynamicRepository, employeeApi);
        }
    }
}
