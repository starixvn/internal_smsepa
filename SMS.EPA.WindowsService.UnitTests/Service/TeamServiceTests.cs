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

    public class TeamServiceTests
    {
        private Fixture fixture = new Fixture();
        private IDynamicRepository dynamicRepository;
        private ITeamAPI teamApi;

        [Fact]
        public void GetTeams_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var criteria = fixture.Create<GetTeamCriteria>();
            var expected = fixture.Create<IEnumerable<Team>>();
            var service = GetService();
            dynamicRepository.Fetch<Team>(criteria).Returns(expected);

            // Act
            var teams = service.GetTeams(criteria.FromDate);

            // Assert
            Assert.Equal(expected, teams);
        }

        [Fact]
        public async Task GetLastModifiedDateAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var token = fixture.Create<TokenAccess>();
            var service = GetService();
            var expected = fixture.Create<DateTime>();
            teamApi.GetLastModifiedDate(token).Returns(expected);

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
            var teams = fixture.Create<IEnumerable<Team>>();
            var expected = fixture.Create<CommandResult>();
            var service = GetService();
            teamApi.UpdateAsync(teams, token).Returns(expected);

            // Act
            var actual = service.UpdateAsync(teams, token);

            // Assert
            Assert.Equal(expected, actual.Result);
        }

        private TeamService GetService()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            teamApi = Substitute.For<ITeamAPI>();
            return new TeamService(dynamicRepository, teamApi);
        }
    }
}
