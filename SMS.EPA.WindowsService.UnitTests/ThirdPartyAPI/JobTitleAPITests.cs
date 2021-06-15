namespace SMS.EPA.WindowsService.UnitTests.ThirdPartyAPI
{
    using AutoFixture;
    using Newtonsoft.Json;
    using NSubstitute;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Model.API;
    using SMS.EPA.WindowsService.Model;
    using SMS.EPA.WindowsService.ThirdPartyAPI;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using Xunit;

    public class JobTitleAPITests
    {
        private readonly Fixture fixture = new Fixture();

        [Fact]
        public void UpdateAsync_WhenCalled_ReturnExpectedResult()
        {
            //Arrange
            var jobTitles = fixture.Create<IEnumerable<JobTitle>>();
            var token = fixture.Create<TokenAccess>();
            ApplicationConfig.ServiceAPI = "http://localhost:63238/";
            var httpClientFactoryMock = Substitute.For<IHttpClientFactory>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(fixture.Create<CommandResult>()))
            });
            var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);

            httpClientFactoryMock.CreateClient().Returns(fakeHttpClient);

            // Act
            var service = new JobTitleAPI(httpClientFactoryMock);
            var result = service.UpdateAsync(jobTitles, token);

            // Assert
            Assert.Equal(result.Result.Code, (int)HttpStatusCode.OK);
        }
    }
}
