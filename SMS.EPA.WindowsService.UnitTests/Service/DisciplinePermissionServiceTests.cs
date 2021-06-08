namespace SMS.EPA.WindowsService.UnitTests.Service
{
    using AutoFixture;
    using Fanex.Data.Repository;
    using NSubstitute;
    using SMS.EPA.Model.API;
    using SMS.EPA.WindowsService.Model;
    using SMS.EPA.WindowsService.Services;
    using SMS.EPA.WindowsService.ThirdPartyAPI;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class DisciplinePermissionServiceTests
    {
        private Fixture fixture = new Fixture();
        private IDynamicRepository dynamicRepository;
        private IEmployeeSupervisorAPI employeeSupervisorApi;
        private ITeamSupervisorAPI teamSupervisorApi;
        private ITokenAPI tokenApi;

        [Fact]
        public void GetLastModifiedDateForEmployeeAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var info = new ApplicationInfo { UserName = "aenfJVK95Xu2Znh8", Password = "aenfJVK95Xu2Znh8" };
            var token = GetTokenService().Request(info);
            var service = GetService();

            // Act
            var actual = service.GetLastModifiedDateForEmployeeAsync(token.Result);

            // Assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void GetLastModifiedDateForGroup_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var info = new ApplicationInfo { UserName = "aenfJVK95Xu2Znh8", Password = "aenfJVK95Xu2Znh8" };
            var token = GetTokenService().Request(info);
            var service = GetService();

            // Act
            var actual = service.GetLastModifiedDateForGroup(token.Result);

            // Assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void GetDepartments_WhenCalledInputValid_ReturnExpectedResult()
        {
            // Arrange
            var fromDate = new DateTime(1900, 01, 01);
            var service = GetService();

            // Act
            var employees = service.GetEmployees(fromDate);

            // Assert
            Assert.NotNull(employees);
        }

        [Fact]
        public void GetGroups_WhenCalledInputValid_ReturnExpectedResult()
        {
            // Arrange
            var fromDate = new DateTime(1900, 01, 01);
            var service = GetService();

            // Act
            var groups = service.GetGroups(fromDate);

            // Assert
            Assert.NotNull(groups);
        }

        [Fact]
        public void UpdateEmployeesAsync_WhenCalledInputValid_ReturnExpectedResult()
        {
            // Arrange
            var info = new ApplicationInfo { UserName = "aenfJVK95Xu2Znh8", Password = "aenfJVK95Xu2Znh8" };
            var token = GetTokenService().Request(info);
            var employees = fixture.Create<IEnumerable<EmployeeDisciplinePermission>>();
            var service = GetService();

            // Act
            var actual = service.UpdateEmployeesAsync(employees, token.Result);

            // Assert
            Assert.True(actual.Result.IsSuccessful);
        }

        [Fact]
        public void UpdateGroupsAsync_WhenCalledInputValid_ReturnExpectedResult()
        {
            // Arrange
            var info = new ApplicationInfo { };
            var token = GetTokenService().Request(info);
            var groups = fixture.Create<IEnumerable<TeamDisciplinePermission>>();
            var service = GetService();

            // Act
            var actual = service.UpdateGroupsAsync(groups, token.Result);

            // Assert
            Assert.False(actual.Result.IsSuccessful);
        }

        private DisciplinePermissionService GetService()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            employeeSupervisorApi = Substitute.For<IEmployeeSupervisorAPI>();
            teamSupervisorApi = Substitute.For<ITeamSupervisorAPI>();
            return new DisciplinePermissionService(dynamicRepository, employeeSupervisorApi, teamSupervisorApi);
        }

        private TokenService GetTokenService()
        {
            tokenApi = Substitute.For<ITokenAPI>();
            return new TokenService(tokenApi);
        }
    }
}
