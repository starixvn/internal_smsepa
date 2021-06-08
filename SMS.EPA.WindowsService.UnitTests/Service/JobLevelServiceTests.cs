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

    public class JobLevelServiceTests
    {
        private Fixture fixture = new Fixture();
        private IDynamicRepository dynamicRepository;
        private IJobLevelAPI jobLevelApi;
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
        public void GetJobLevel_WhenCalledInputValid_ReturnExpectedResult()
        {
            // Arrange
            var service = GetService();

            // Act
            var jobLevels = service.GetJobLevel(null);

            // Assert
            Assert.NotNull(jobLevels);
        }

        [Fact]
        public void GetJobLevel_WhenCalledInputInValid_ReturnExpectedResult()
        {
            // Arrange
            var fromDate = new DateTime(9999, 01, 01);
            var service = GetService();

            // Act
            var jobLevels = service.GetJobLevel(fromDate);

            // Assert
            Assert.Null(jobLevels);
        }

        [Fact]
        public void Update_WhenCalledInputValid_ReturnExpectedResult()
        {
            // Arrange
            var info = new ApplicationInfo { UserName = "aenfJVK95Xu2Znh8", Password = "aenfJVK95Xu2Znh8" };
            var token = GetTokenService().Request(info);
            var jobLevels = fixture.Create<IEnumerable<JobLevel>>();
            var service = GetService();

            // Act
            var actual = service.Update(jobLevels, token.Result);

            // Assert
            Assert.True(actual.Result.IsSuccessful);
        }

        [Fact]
        public void Update_WhenCalledInputInValid_ReturnExpectedResult()
        {
            // Arrange
            var info = new ApplicationInfo { };
            var token = GetTokenService().Request(info);
            var jobLevels = fixture.Create<IEnumerable<JobLevel>>();
            var service = GetService();

            // Act
            var actual = service.Update(jobLevels, token.Result);

            // Assert
            Assert.False(actual.Result.IsSuccessful);
        }

        private JobLevelService GetService()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            jobLevelApi = Substitute.For<IJobLevelAPI>();
            return new JobLevelService(dynamicRepository, jobLevelApi);
        }

        private TokenService GetTokenService()
        {
            tokenApi = Substitute.For<ITokenAPI>();
            return new TokenService(tokenApi);
        }
    }
}
