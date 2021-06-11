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

    public class JobLevelAPITests
    {
        private readonly Fixture fixture = new Fixture();

        [Fact]
        public void GetLastModifiedDate_WhenCalled_ReturnExpectedResult()
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
            var token = fixture.Create<TokenAccess>();

            // Act
            var service = new JobLevelAPI(mockFactory.Object);
            var result = service.GetLastModifiedDate(token);

            //Assert
            mockHttpMessageHandler.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
               ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public void UpdateAsync_WhenCalled_ReturnExpectedResult()
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
            var token = fixture.Create<TokenAccess>();
            var jobLevel = fixture.Create<IEnumerable<JobLevel>>();

            // Act
            var service = new JobLevelAPI(mockFactory.Object);
            var result = service.UpdateAsync(jobLevel, token);

            //Assert
            Assert.Equal(result.Result.Code, (int)HttpStatusCode.OK);
            mockHttpMessageHandler.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
               ItExpr.IsAny<CancellationToken>());
        }
    }
}
