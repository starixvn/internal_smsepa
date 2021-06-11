namespace SMS.EPA.WindowsService.UnitTests.ThirdPartyAPI
{
    using AutoFixture;
    using Moq;
    using Moq.Protected;
    using SMS.EPA.Model.API;
    using SMS.EPA.WindowsService.Model;
    using SMS.EPA.WindowsService.ThirdPartyAPI;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class TokenAPITests
    {
        private readonly Fixture fixture = new Fixture();

        [Fact]
        public void RequestAsync_WhenCalled_ReturnExpectedResult()
        {
            //Arrange
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(fixture.Create<string>()),
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
            var info = fixture.Create< ApplicationInfo>();

            // Act
            var service = new TokenAPI(mockFactory.Object);
            var result = service.RequestAsync(info);

            //Assert
            mockHttpMessageHandler.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
               ItExpr.IsAny<CancellationToken>());
        }
    }
}
