namespace SMS.EPA.WindowsService.UnitTests.Service
{
    using AutoFixture;
    using Fanex.Data.Repository;
    using NSubstitute;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Model.API;
    using SMS.EPA.WindowsService.Criteria;
    using SMS.EPA.WindowsService.Model;
    using SMS.EPA.WindowsService.Services;
    using SMS.EPA.WindowsService.ThirdPartyAPI;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class DisciplineSummaryServiceTests
    {
        private Fixture fixture = new Fixture();
        private IDynamicRepository dynamicRepository;
        private IDisciplineSummaryAPI attendanceApi;

        [Fact]
        public void GetDisciplineSummary_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var criteria = fixture.Create<GetDisciplineSummaryCriteria>();
            var expected = fixture.Create<IEnumerable<DisciplineSummary>>();
            var service = GetService();
            dynamicRepository.Fetch<DisciplineSummary>(criteria).Returns(expected);

            // Act
            var disciplines = service.GetDisciplineSummary(criteria.LastModifiedDate);

            // Assert
            Assert.Equal(expected, disciplines);
        }

        [Fact]
        public async Task GetLastModifiedDateAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var token = fixture.Create<TokenAccess>();
            var service = GetService();
            var expected = fixture.Create<DateTime>();
            attendanceApi.GetLastModifiedDateAsync(token).Returns(expected);

            // Act
            var actual = await service.GetLastModifiedDateAsync(token);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Update_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var token = fixture.Create<TokenAccess>();
            var disciplines = fixture.Create<IEnumerable<DisciplineSummary>>();
            var expected = fixture.Create<CommandResult>();
            var service = GetService();
            attendanceApi.UpdateAsync(disciplines, token).Returns(expected);

            // Act
            var actual = service.UpdateAsync(disciplines, token);

            // Assert
            Assert.Equal(expected, actual.Result);
        }

        private DisciplineSummaryService GetService()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            attendanceApi = Substitute.For<IDisciplineSummaryAPI>();
            return new DisciplineSummaryService(dynamicRepository, attendanceApi);
        }
        
    }
}
