namespace SMS.EPA.WindowsService.UnitTests.Service
{
    using AutoFixture;
    using Fanex.Data.Repository;
    using NSubstitute;
    using SMS.EPA.Model.API;
    using SMS.EPA.WindowsService.Model;
    using SMS.EPA.WindowsService.Services;
    using SMS.EPA.WindowsService.ThirdPartyAPI;
    using System.Collections.Generic;
    using Xunit;

    public class JobTitleServiceTests
    {
        private Fixture fixture = new Fixture();
        private IDynamicRepository dynamicRepository;
        private IJobTitleAPI jobTitleApi;
        private ITokenAPI tokenApi;

        [Fact]
        public void GetDepartments_WhenCalledInputInValid_ReturnExpectedResult()
        {
            // Arrange
            var service = GetService();

            // Act
            var jobTitles = service.GetJobTitles();

            // Assert
            Assert.NotNull(jobTitles);
        }

        [Fact]
        public void UpdateAsync_WhenCalledInputValid_ReturnExpectedResult()
        {
            // Arrange
            var info = new ApplicationInfo { UserName = "aenfJVK95Xu2Znh8", Password = "aenfJVK95Xu2Znh8" };
            var token = GetTokenService().Request(info);
            var jobTitles = fixture.Create<IEnumerable<JobTitle>>();
            var service = GetService();

            // Act
            var actual = service.UpdateAsync(jobTitles, token.Result);

            // Assert
            Assert.True(actual.Result.IsSuccessful);
        }

        [Fact]
        public void UpdateAsync_WhenCalledInputInValid_ReturnExpectedResult()
        {
            // Arrange
            var info = new ApplicationInfo { };
            var token = GetTokenService().Request(info);
            var jobTitles = fixture.Create<IEnumerable<JobTitle>>();
            var service = GetService();

            // Act
            var actual = service.UpdateAsync(jobTitles, token.Result);

            // Assert
            Assert.False(actual.Result.IsSuccessful);
        }

        private JobTitleService GetService()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            jobTitleApi = Substitute.For<IJobTitleAPI>();
            return new JobTitleService(dynamicRepository, jobTitleApi);
        }

        private TokenService GetTokenService()
        {
            tokenApi = Substitute.For<ITokenAPI>();
            return new TokenService(tokenApi);
        }
    }
}
