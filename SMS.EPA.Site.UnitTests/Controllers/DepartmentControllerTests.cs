namespace SMS.EPA.Site.UnitTests.Controllers
{
    using Criteria;
    using Model;
    using NSubstitute;
    using NUnit.Framework;
    using Services.Interfaces;
    using Site.Controllers.API;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Reviewed")]
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

        [Test]
        public void GetDepartment_AnyArg_ReturnAsExpectedType()
        {
            // Arrange
            _departmentController = new DepartmentController();

            // Act
            var actual = _departmentController.GetType().GetMethod("GetDepartment").ReturnType;

            // Assert
            Assert.AreEqual(typeof(DepartmentInfoModel).Name, actual.Name);
        }
    }
}