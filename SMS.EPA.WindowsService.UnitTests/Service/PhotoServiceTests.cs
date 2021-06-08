namespace SMS.EPA.WindowsService.UnitTests.Service
{
    using AutoFixture;
    using NSubstitute;
    using SMS.EPA.Model.API;
    using SMS.EPA.WindowsService.Model;
    using SMS.EPA.WindowsService.Services;
    using SMS.EPA.WindowsService.ThirdPartyAPI;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class PhotoServiceTests
    {
        private Fixture fixture = new Fixture();
        private IPhotoAPI photoApi;
        private ITokenAPI tokenApi;

        [Fact]
        public void GetLastModifiedDate_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var info = new ApplicationInfo { UserName = "aenfJVK95Xu2Znh8", Password = "aenfJVK95Xu2Znh8" };
            var token = GetTokenService().Request(info);
            var service = GetService();

            // Act
            var actual = service.GetLastModifiedDate(token.Result);

            // Assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void GetPhotos_WhenCalledInputValid_ReturnExpectedResult()
        {
            // Arrange
            var service = GetService();

            // Act
            var department = service.GetPhotos(null);

            // Assert
            Assert.NotNull(department);
        }

        [Fact]
        public void GetPhotos_WhenCalledInputInValid_ReturnExpectedResult()
        {
            // Arrange
            var fromDate = new DateTime(9999, 01, 01);
            var service = GetService();

            // Act
            var department = service.GetPhotos(fromDate);

            // Assert
            Assert.Null(department);
        }

        [Fact]
        public void UpdateAsync_WhenCalledInputValid_ReturnExpectedResult()
        {
            // Arrange
            var info = new ApplicationInfo { UserName = "aenfJVK95Xu2Znh8", Password = "aenfJVK95Xu2Znh8" };
            var token = GetTokenService().Request(info);
            var photo = fixture.Create<IEnumerable<Photo>>();
            var service = GetService();

            // Act
            var actual = service.UpdateAsync(photo, token.Result);

            // Assert
            Assert.True(actual.Result.IsSuccessful);
        }

        [Fact]
        public void UpdateAsync_WhenCalledInputInValid_ReturnExpectedResult()
        {
            // Arrange
            var info = new ApplicationInfo { };
            var token = GetTokenService().Request(info);
            var photo = fixture.Create<IEnumerable<Photo>>();
            var service = GetService();

            // Act
            var actual = service.UpdateAsync(photo, token.Result);

            // Assert
            Assert.False(actual.Result.IsSuccessful);
        }

        private PhotoService GetService()
        {
            photoApi = Substitute.For<IPhotoAPI>();
            return new PhotoService(photoApi);
        }

        private TokenService GetTokenService()
        {
            tokenApi = Substitute.For<ITokenAPI>();
            return new TokenService(tokenApi);
        }
    }
}
