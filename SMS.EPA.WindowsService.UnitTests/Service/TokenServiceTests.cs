namespace SMS.EPA.WindowsService.UnitTests.Service
{
    using AutoFixture;
    using NSubstitute;
    using SMS.EPA.WindowsService.Model;
    using SMS.EPA.WindowsService.Services;
    using SMS.EPA.WindowsService.ThirdPartyAPI;
    using System.Threading.Tasks;
    using Xunit;

    public class TokenServiceTests
    {
        private Fixture fixture = new Fixture();
        private ITokenAPI tokenApi;

        [Fact]
        public async Task GetTeams_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var info = fixture.Create<ApplicationInfo>();
            var expected = fixture.Create<TokenAccess>();
            var service = GetService();
            tokenApi.RequestAsync(info).Returns(expected);

            // Act
            var token = await service.Request(info);

            // Assert
            Assert.Equal(expected, token);
        }

        private TokenService GetService()
        {
            tokenApi = Substitute.For<ITokenAPI>();
            return new TokenService(tokenApi);
        }
    }
}
