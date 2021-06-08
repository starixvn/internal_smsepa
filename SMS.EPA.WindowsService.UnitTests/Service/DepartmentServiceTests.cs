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
    public class DepartmentServiceTests
    {
        private Fixture fixture = new Fixture();
        private IDynamicRepository dynamicRepository;
        private IDepartmentAPI departmentApi;
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
        public void GetDepartments_WhenCalledInputValid_ReturnExpectedResult()
        {
            // Arrange
            var service = GetService();

            // Act
            var department = service.GetDepartments(null);

            // Assert
            Assert.NotNull(department);
        }

        [Fact]
        public void GetDepartments_WhenCalledInputInValid_ReturnExpectedResult()
        {
            // Arrange
            var fromDate = new DateTime(9999, 01, 01);
            var service = GetService();

            // Act
            var department = service.GetDepartments(fromDate);

            // Assert
            Assert.Null(department);
        }

        [Fact]
        public void Update_WhenCalledInputValid_ReturnExpectedResult()
        {
            // Arrange
            var info = new ApplicationInfo { UserName = "aenfJVK95Xu2Znh8", Password = "aenfJVK95Xu2Znh8" };
            var token = GetTokenService().Request(info);
            var departments = fixture.Create<IEnumerable<Department>>();
            var service = GetService();

            // Act
            var actual = service.Update(departments, token.Result);

            // Assert
            Assert.True(actual.Result.IsSuccessful);
        }

        [Fact]
        public void Update_WhenCalledInputInValid_ReturnExpectedResult()
        {
            // Arrange
            var info = new ApplicationInfo { };
            var token = GetTokenService().Request(info);
            var departments = fixture.Create<IEnumerable<Department>>();
            var service = GetService();

            // Act
            var actual = service.Update(departments, token.Result);

            // Assert
            Assert.False(actual.Result.IsSuccessful);
        }

        private DepartmentService GetService()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            departmentApi = Substitute.For<IDepartmentAPI>();
            return new DepartmentService(dynamicRepository, departmentApi);
        }

        private TokenService GetTokenService()
        {
            tokenApi = Substitute.For<ITokenAPI>();
            return new TokenService(tokenApi);
        }
    }
}
