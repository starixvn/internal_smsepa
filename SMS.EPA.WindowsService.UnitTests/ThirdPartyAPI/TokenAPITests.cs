namespace SMS.EPA.WindowsService.UnitTests.ThirdPartyAPI
{
    using AutoFixture;
    using Newtonsoft.Json;
    using NSubstitute;
    using SMS.EPA.WindowsService.Model;
    using SMS.EPA.WindowsService.ThirdPartyAPI;
    using System.Net.Http;
    using Xunit;

    public class TokenAPITests
    {
        private readonly Fixture fixture = new Fixture();

        [Fact]
        public void RequestAsync_WhenCalled_ReturnExpectedResult()
        {
            //Arrange
            var info = fixture.Create<ApplicationInfo>();
            var token = fixture.Create<TokenAccess>();
            token.Issued = "Mon, 14 Jun 2021 07:24:55 GMT";
            token.Expires = "Mon, 14 Jun 2021 08:24:55 GMT";
            ApplicationConfig.ServiceAPI = "http://localhost:63238/";
            var httpClientFactoryMock = Substitute.For<IHttpClientFactory>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(token))
            });
            var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
            httpClientFactoryMock.CreateClient().Returns(fakeHttpClient);

            // Act
            var service = new TokenAPI(httpClientFactoryMock);
            var result = service.RequestAsync(info);

            // Assert
            Assert.Equal(result.Result.Value, token.Value);
        }
    }
}
