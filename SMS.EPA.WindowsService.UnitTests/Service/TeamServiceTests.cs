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

    public class TeamServiceTests
    {
        private Fixture fixture = new Fixture();
        private IDynamicRepository dynamicRepository;
        private ITeamAPI teamApi;
        private ITokenAPI tokenApi;

        [Fact]
        public void GetLastModifiedDateAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var info = new ApplicationInfo { UserName = "aenfJVK95Xu2Znh8", Password = "aenfJVK95Xu2Znh8" };
            var token = GetTokenService().Request(info);
            var service = GetService();

            // Act
            var actual = service.GetLastModifiedDateAsync(token.Result);

            // Assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void GetTeams_WhenCalledInputValid_ReturnExpectedResult()
        {
            // Arrange
            var service = GetService();

            // Act
            var teams = service.GetTeams(null);

            // Assert
            Assert.NotNull(teams);
        }

        [Fact]
        public void GetTeams_WhenCalledInputInValid_ReturnExpectedResult()
        {
            // Arrange
            var fromDate = new DateTime(9999, 01, 01);
            var service = GetService();

            // Act
            var teams = service.GetTeams(fromDate);

            // Assert
            Assert.Null(teams);
        }

        [Fact]
        public void UpdateAsync_WhenCalledInputValid_ReturnExpectedResult()
        {
            // Arrange
            var info = new ApplicationInfo { UserName = "aenfJVK95Xu2Znh8", Password = "aenfJVK95Xu2Znh8" };
            var token = GetTokenService().Request(info);
            var teams = fixture.Create<IEnumerable<Team>>();
            var service = GetService();

            // Act
            var actual = service.UpdateAsync(teams, token.Result);

            // Assert
            Assert.True(actual.Result.IsSuccessful);
        }

        [Fact]
        public void UpdateAsync_WhenCalledInputInValid_ReturnExpectedResult()
        {
            // Arrange
            var info = new ApplicationInfo { };
            var token = GetTokenService().Request(info);
            var teams = fixture.Create<IEnumerable<Team>>();
            var service = GetService();

            // Act
            var actual = service.UpdateAsync(teams, token.Result);

            // Assert
            Assert.False(actual.Result.IsSuccessful);
        }

        private TeamService GetService()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            teamApi = Substitute.For<ITeamAPI>();
            return new TeamService(dynamicRepository, teamApi);
        }

        private TokenService GetTokenService()
        {
            tokenApi = Substitute.For<ITokenAPI>();
            return new TokenService(tokenApi);
        }
    }
}
