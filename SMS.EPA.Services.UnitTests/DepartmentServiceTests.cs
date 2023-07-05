namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Repositories;

    [TestFixture]
    public class DepartmentServiceTests
    {
        private IDepartmentRepository departmentRepository;
        private IDepartmentService departmentService;

        [Test]
        public void GetDepartment_VaidArg_ReturnInstanceOfListOfDepartmentModel()
        {
            // Arrange
            departmentService = new DepartmentService();

            // Act
            var actual = departmentService.GetDepartment(Arg.Any<GetDepartmentCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IList<Department>), actual);
        }

        [Test]
        public void GetDepartment_VaidArg_ReturnExpectedDepartments()
        {
            // Arrange
            var criteria = new GetDepartmentCriteria();
            var expectedDepartments = new List<Department> { new Department() };
            ArrangeGetDepartment(expectedDepartments);

            // Act
            var actual = departmentService.GetDepartment(criteria);

            // Assert
            Assert.AreEqual(expectedDepartments, actual);
        }

        [Test]
        public void GetDepartment_InVaidArg_ReturnDefaultDepartments()
        {
            // Arrange
            GetDepartmentCriteria criteria = null;
            var expectedDepartments = new List<Department> { new Department() };
            ArrangeGetDepartment(expectedDepartments);

            // Act
            var actual = departmentService.GetDepartment(criteria);

            // Assert
            Assert.AreNotEqual(expectedDepartments, actual);
        }

        private static IDepartmentRepository FakeDepartmentRepository()
        {
            var repository = Substitute.For<IDepartmentRepository>();

            return repository;
        }

        private void ArrangeGetDepartment(IList<Department> expectedDepartments = null)
        {
            departmentRepository = FakeDepartmentRepository();
            departmentRepository.GetDepartment(Arg.Any<GetDepartmentCriteria>()).Returns(expectedDepartments ?? new List<Department>());
            departmentService = new DepartmentService(departmentRepository);
        }
    }
}