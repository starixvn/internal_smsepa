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
    public class DepartmentServiceTests
    {
        private readonly Fixture fixture = new Fixture();
        private IDynamicRepository dynamicRepository;
        private IDepartmentAPI departmentApi;

        [Fact]
        public void GetDepartments_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var criteria = fixture.Create<GetDepartmentCriteria>();
            var expected = fixture.Create<IEnumerable<Department>>();
            var service = GetService();
            dynamicRepository.Fetch<Department>(Arg.Do<GetDepartmentCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var department = service.GetDepartments(criteria.FromDate);

            // Assert
            Assert.Equal(expected, department);
        }

        [Fact]
        public async Task GetLastModifiedDateAsync_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var token = fixture.Create<TokenAccess>();
            var service = GetService();
            var expected = fixture.Create<DateTime>();
            departmentApi.GetLastModifiedDate(token).Returns(expected);

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
            var departments = fixture.Create<IEnumerable<Department>>();
            var expected = fixture.Create<CommandResult>();
            var service = GetService();
            departmentApi.UpdateAsync(departments, token).Returns(expected);

            // Act
            var actual = service.Update(departments, token);

            // Assert
            Assert.Equal(expected, actual.Result);
        }

        private DepartmentService GetService()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            departmentApi = Substitute.For<IDepartmentAPI>();
            return new DepartmentService(dynamicRepository, departmentApi);
        }
    }
}
