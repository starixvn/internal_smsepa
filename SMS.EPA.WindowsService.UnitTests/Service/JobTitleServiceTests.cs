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
    using System.Collections.Generic;
    using Xunit;

    public class JobTitleServiceTests
    {
        private Fixture fixture = new Fixture();
        private IDynamicRepository dynamicRepository;
        private IJobTitleAPI jobTitleApi;

        [Fact]
        public void GetJobTitles_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var criteria = fixture.Create<GetJobTitleCriteria>();
            var expected = fixture.Create<IEnumerable<JobTitle>>();
            var service = GetService();
            dynamicRepository.Fetch<JobTitle>(criteria).Returns(expected);

            // Act
            var jobTitles = service.GetJobTitles();

            // Assert
            Assert.Equal(expected, jobTitles);
        }

        [Fact]
        public void UpdateAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var token = fixture.Create<TokenAccess>();
            var jobTitles = fixture.Create<IEnumerable<JobTitle>>();
            var expected = fixture.Create<CommandResult>();
            var service = GetService();
            jobTitleApi.UpdateAsync(jobTitles, token).Returns(expected);

            // Act
            var actual = service.UpdateAsync(jobTitles, token);

            // Assert
            Assert.Equal(expected, actual.Result);
        }

        private JobTitleService GetService()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            jobTitleApi = Substitute.For<IJobTitleAPI>();
            return new JobTitleService(dynamicRepository, jobTitleApi);
        }
    }
}
