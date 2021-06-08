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

    public class DisciplinePermissionServiceTests
    {
        private Fixture fixture = new Fixture();
        private IDynamicRepository dynamicRepository;
        private IEmployeeSupervisorAPI employeeSupervisorApi;
        private ITeamSupervisorAPI teamSupervisorApi;

        [Fact]
        public async Task GetLastModifiedDateForGroup_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var token = fixture.Create<TokenAccess>();
            var service = GetService();
            var expected = fixture.Create<DateTime>();
            teamSupervisorApi.GetLastModifiedDateAsync(token).Returns(expected);

            // Act
            var actual = await service.GetLastModifiedDateForGroup(token);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetLastModifiedDateForEmployeeAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var token = fixture.Create<TokenAccess>();
            var service = GetService();
            var expected = fixture.Create<DateTime>();
            employeeSupervisorApi.GetLastModifiedDateAsync(token).Returns(expected);

            // Act
            var actual = await service.GetLastModifiedDateForEmployeeAsync(token);

            // Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void GetEmployees_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var fromDate = fixture.Create<DateTime>();
            var criteria = fixture.Create<GetEmployeeDisciplinePermissionCriteria>();
            var expected = fixture.Create<IEnumerable<EmployeeDisciplinePermission>>();
            var service = GetService();
            dynamicRepository.Fetch<EmployeeDisciplinePermission>(criteria).Returns(expected);

            // Act
            var groups = service.GetEmployees(fromDate);

            // Assert
            Assert.Equal(expected, groups);
        }

        [Fact]
        public void GetGroups_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var fromDate = fixture.Create<DateTime>();
            var criteria = fixture.Create<GetGroupDisciplinePermissionCriteria>();
            var expected = fixture.Create<IEnumerable<TeamDisciplinePermission>>();
            var service = GetService();
            dynamicRepository.Fetch<TeamDisciplinePermission>(criteria).Returns(expected);

            // Act
            var groups = service.GetGroups(fromDate);

            // Assert
            Assert.Equal(expected, groups);
        }

        [Fact]
        public async Task UpdateEmployeesAsync_WhenCalled_ReturnExpectedResultAsync()
        {
            // Arrange
            var token = fixture.Create<TokenAccess>();
            var employees = fixture.Create<IEnumerable<EmployeeDisciplinePermission>>();
            var expected = fixture.Create<CommandResult>();
            var service = GetService();
            employeeSupervisorApi.UpdateAsync(employees, token).Returns(expected);

            // Act
            var actual = await service.UpdateEmployeesAsync(employees, token);

            // Assert
            Assert.Equal(actual, expected);
        }

        [Fact]
        public async Task UpdateGroupsAsync_WhenCalled_ReturnExpectedResultAsync()
        {
            // Arrange
            var token = fixture.Create<TokenAccess>();
            var groups = fixture.Create<IEnumerable<TeamDisciplinePermission>>();
            var expected = fixture.Create<CommandResult>();
            var service = GetService();
            teamSupervisorApi.Update(groups, token).Returns(expected);

            // Act
            var actual = await service.UpdateGroupsAsync(groups, token);

            // Assert
            Assert.Equal(actual, expected);
        }

        private DisciplinePermissionService GetService()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            employeeSupervisorApi = Substitute.For<IEmployeeSupervisorAPI>();
            teamSupervisorApi = Substitute.For<ITeamSupervisorAPI>();
            return new DisciplinePermissionService(dynamicRepository, employeeSupervisorApi, teamSupervisorApi);
        }
    }
}
