namespace SMS.EPA.WindowsService.UnitTests.Service
{
    using NSubstitute;
    using SMS.EPA.WindowsService.Model;
    using SMS.EPA.WindowsService.Services;
    using SMS.EPA.WindowsService.ThirdPartyAPI;
    using Xunit;

    public class TokenServiceTests
    {
        private ITokenAPI tokenApi;

        [Fact]
        public void Request_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var info = new ApplicationInfo { UserName = "aenfJVK95Xu2Znh8", Password= "aenfJVK95Xu2Znh8" };
            var service = GetService();

            // Act
            var actual = service.Request(info);

            // Assert
            Assert.NotNull(actual.Result.Value);
            Assert.NotNull(actual.Result.Type);
            Assert.NotNull(actual.Result.Expires);
        }

        [Fact]
        public void Request_WhenCalledInputInvalid_ReturnExpectedResult()
        {
            // Arrange
            var info = new ApplicationInfo { UserName = "123", Password = "123" };
            var service = GetService();

            // Act
            var actual = service.Request(info);

            // Assert
            Assert.Null(actual.Result.Value);
            Assert.Null(actual.Result.Type);
            Assert.Null(actual.Result.Expires);
        }

        private TokenService GetService()
        {
            tokenApi = Substitute.For<ITokenAPI>();
            return new TokenService(tokenApi);
        }
    }
}
