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

    public class DisciplineSummaryServiceTests
    {
        private Fixture fixture = new Fixture();
        private IDynamicRepository dynamicRepository;
        private IDisciplineSummaryAPI attendanceApi;
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
        public void GetDisciplineSummary_WhenCalledInputInValid_ReturnExpectedResult()
        {
            // Arrange
            var lastModifiedDate = new DateTime(9999, 01, 01);
            var service = GetService();

            // Act
            var disciplineSummary = service.GetDisciplineSummary(lastModifiedDate);

            // Assert
            Assert.Null(disciplineSummary);
        }

        [Fact]
        public void GetDisciplineSummary_WhenCalledInputValid_ReturnExpectedResult()
        {
            // Arrange
            var fromDate = new DateTime(1900, 01, 01);
            var service = GetService();

            // Act
            var disciplineSummary = service.GetDisciplineSummary(fromDate);

            // Assert
            Assert.NotNull(disciplineSummary);
        }

        [Fact]
        public void UpdateAsync_WhenCalledInputValid_ReturnExpectedResult()
        {
            // Arrange
            var info = new ApplicationInfo { UserName = "aenfJVK95Xu2Znh8", Password = "aenfJVK95Xu2Znh8" };
            var token = GetTokenService().Request(info);
            var disciplineSummary = fixture.Create<IEnumerable<DisciplineSummary>>();
            var service = GetService();

            // Act
            var actual = service.UpdateAsync(disciplineSummary, token.Result);

            // Assert
            Assert.True(actual.Result.IsSuccessful);
        }

        [Fact]
        public void UpdateAsync_WhenCalledInputInValid_ReturnExpectedResult()
        {
            // Arrange
            var info = new ApplicationInfo { };
            var token = GetTokenService().Request(info);
            var disciplineSummary = fixture.Create<IEnumerable<DisciplineSummary>>();
            var service = GetService();

            // Act
            var actual = service.UpdateAsync(disciplineSummary, token.Result);

            // Assert
            Assert.False(actual.Result.IsSuccessful);
        }

        private DisciplineSummaryService GetService()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            attendanceApi = Substitute.For<IDisciplineSummaryAPI>();
            return new DisciplineSummaryService(dynamicRepository, attendanceApi);
        }

        private TokenService GetTokenService()
        {
            tokenApi = Substitute.For<ITokenAPI>();
            return new TokenService(tokenApi);
        }
    }
}
