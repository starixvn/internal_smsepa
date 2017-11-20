namespace SMS.EPA.Site.UnitTests
{
    using Criteria;
    using Model;
    using NSubstitute;
    using NUnit.Framework;
    using Services.Interfaces;
    using Site.Controllers.API;

    [TestFixture]
    public class DepartmentControllerTests
    {
        private ICommonService _commonService;
        private DepartmentController _departmentController;

        [Test]
        public void GetDepartment_AnyArg_ReturnExpectedDepartment()
        {
            // Arrange
            var expectedDepartment = new DepartmentInfoModel();
            _commonService = Substitute.For<ICommonService>();
            _commonService.GetDepartment(Arg.Any<GetDepartmentCriteria>()).Returns(expectedDepartment);
            _departmentController = new DepartmentController(_commonService);

            // Act
            var actual = _departmentController.GetDepartment();

            // Assert
            Assert.AreEqual(expectedDepartment, actual);
        }
    }
}