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

    public class JobLevelServiceTests
    {
        private Fixture fixture = new Fixture();
        private IDynamicRepository dynamicRepository;
        private IJobLevelAPI jobLevelApi;

        [Fact]
        public void GetJobLevel_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var criteria = fixture.Create<GetJobLevelCriteria>();
            var expected = fixture.Create<IEnumerable<JobLevel>>();
            var service = GetService();
            dynamicRepository.Fetch<JobLevel>(criteria).Returns(expected);

            // Act
            var employees = service.GetJobLevel(criteria.FromDate);

            // Assert
            Assert.Equal(expected, employees);
        }

        [Fact]
        public async Task GetLastModifiedDateAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var token = fixture.Create<TokenAccess>();
            var service = GetService();
            var expected = fixture.Create<DateTime>();
            jobLevelApi.GetLastModifiedDate(token).Returns(expected);

            // Act
            var actual = await service.GetLastModifiedDateAsync(token);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UpdateAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var token = fixture.Create<TokenAccess>();
            var jobLevels = fixture.Create<IEnumerable<JobLevel>>();
            var expected = fixture.Create<CommandResult>();
            var service = GetService();
            jobLevelApi.UpdateAsync(jobLevels, token).Returns(expected);

            // Act
            var actual = service.Update(jobLevels, token);

            // Assert
            Assert.Equal(expected, actual.Result);
        }

        private JobLevelService GetService()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            jobLevelApi = Substitute.For<IJobLevelAPI>();
            return new JobLevelService(dynamicRepository, jobLevelApi);
        }
    }
}
