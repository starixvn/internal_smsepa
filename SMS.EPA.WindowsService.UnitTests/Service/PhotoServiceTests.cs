namespace SMS.EPA.WindowsService.UnitTests.Service
{
    using AutoFixture;
    using NSubstitute;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Model.API;
    using SMS.EPA.WindowsService.Model;
    using SMS.EPA.WindowsService.Services;
    using SMS.EPA.WindowsService.ThirdPartyAPI;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class PhotoServiceTests
    {
        private readonly Fixture fixture = new Fixture();
        private IPhotoAPI photoApi;

        [Fact]
        public async Task GetLastModifiedDate_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var token = fixture.Create<TokenAccess>();
            var service = GetService();
            var expected = fixture.Create<DateTime>();
            photoApi.GetLastModifiedDateAsync(token).Returns(expected);

            // Act
            var actual = await service.GetLastModifiedDate(token);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UpdateAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var token = fixture.Create<TokenAccess>();
            var photos = fixture.Create<IEnumerable<Photo>>();
            var expected = fixture.Create<CommandResult>();
            var service = GetService();
            photoApi.Update(photos, token).Returns(expected);

            // Act
            var actual = service.UpdateAsync(photos, token);

            // Assert
            Assert.Equal(expected, actual.Result);
        }

        private PhotoService GetService()
        {
            photoApi = Substitute.For<IPhotoAPI>();
            return new PhotoService(photoApi);
        }
    }
}
