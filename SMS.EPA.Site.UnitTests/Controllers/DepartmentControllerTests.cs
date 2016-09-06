namespace SMS.EPA.Site.UnitTests.Controllers
{
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Criteria.Department;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Services.Interfaces;
    using SMS.EPA.Site.Controllers.API;

    [TestFixture]
    public class DepartmentControllerTests
    {
        private ICommonService commonService;
        private DepartmentController departmentController;

        [Test]
        public void GetDepartment_AnyArg_ReturnExpectedDepartment()
        {
            // Arrange
            var expectedDepartment = new DepartmentInfoModel();
            commonService = Substitute.For<ICommonService>();
            commonService.GetDepartment(Arg.Any<GetDepartmentCriteria>()).Returns(expectedDepartment);
            departmentController = new DepartmentController(commonService);

            // Act
            var actual = departmentController.GetDepartment();

            // Assert
            Assert.AreEqual(expectedDepartment, actual);
        }

        [Test]
        public void GetDepartment_AnyArg_ReturnAsExpectedType()
        {
            // Arrange
            departmentController = new DepartmentController();

            // Act
            var actual = departmentController.GetType().GetMethod("GetDepartment").ReturnType;

            // Assert
            Assert.AreEqual(typeof(DepartmentInfoModel).Name, actual.Name);
        }
    }
}
