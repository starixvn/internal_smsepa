namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using Criteria;
    using Implement;
    using Model.Discipline;
    using NSubstitute;
    using NUnit.Framework;
    using Repositories;

    [TestFixture]
    public class DisciplineServiceTests
    {
        private IDisciplineRepository disciplineRepository;
        private DisciplineService disciplineService;

        #region GetDisciplineSummary

        [Test]
        public void GetDisciplineSummary_InValidArg_ReturnDefaultObject()
        {
            // Arrange
            ArrangeGetDisciplineSummary();
            var criteria = Substitute.For<GetDisciplineSummaryCriteria>();
            criteria.IsValid().Returns(false);

            // Act
            var actual = disciplineService.GetDisciplineSummary(criteria);

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<DisciplineSummaryModel>), actual);
        }

        [Test]
        public void GetDisciplineSummary_ValidArg_ReturnExpectedEmployee()
        {
            // Arrange
            var expected = new List<DisciplineSummaryModel>
            {
                new DisciplineSummaryModel
                {
                    Employee = new Model.Enitites.Employee { EmployeeId = 1 },
                    LateIn = 2.5M
                }
            };
            ArrangeGetDisciplineSummary(expected);
            var criteria = Substitute.For<GetDisciplineSummaryCriteria>();
            criteria.IsValid().Returns(true);

            // Act
            var actual = disciplineService.GetDisciplineSummary(criteria);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion GetDisciplineSummary

        #region GetDisciplineSummaryForOtherSystem

        [Test]
        public void GetDisciplineSummaryForOtherSystem_InValidArg_ReturnDefaultObject()
        {
            // Arrange
            ArrangeGetDisciplineSummary();
            var criteria = Substitute.For<GetDisciplineSummaryForOtherSystemCriteria>();
            criteria.IsValid().Returns(false);

            // Act
            var actual = disciplineService.GetDisciplineSummaryForOtherSystem(criteria);

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<DisciplineSummaryForOtherSystem>), actual);
        }

        [Test]
        public void GetDisciplineSummaryForOtherSystem_ValidArg_ReturnExpectedEmployee()
        {
            // Arrange
            var expected = new List<DisciplineSummaryForOtherSystem>
            {
                new DisciplineSummaryForOtherSystem
                {
                    Employee = new Model.Enitites.Employee { EmployeeId = 1 },
                    LateIn = 2.5M
                }
            };
            ArrangeGetDisciplineSummaryForOtherSystem(expected);
            var criteria = Substitute.For<GetDisciplineSummaryForOtherSystemCriteria>();
            criteria.IsValid().Returns(true);

            // Act
            var actual = disciplineService.GetDisciplineSummaryForOtherSystem(criteria);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion GetDisciplineSummaryForOtherSystem

        #region Helpers

        private IDisciplineRepository FakeDisciplineRepository()
        {
            var repository = Substitute.For<IDisciplineRepository>();

            return repository;
        }

        private DisciplineService GetDisciplineService(IDisciplineRepository employeeRepository = null)
        {
            var service = new DisciplineService(disciplineRepository);
            return service;
        }

        private void ArrangeGetDisciplineSummary(IEnumerable<DisciplineSummaryModel> result = null)
        {
            disciplineRepository = FakeDisciplineRepository();
            disciplineRepository.GetDisciplineSummary(Arg.Any<GetDisciplineSummaryCriteria>()).Returns(result ?? new List<DisciplineSummaryModel>());
            disciplineService = GetDisciplineService(disciplineRepository);
        }

        private void ArrangeGetDisciplineSummaryForOtherSystem(IEnumerable<DisciplineSummaryForOtherSystem> result = null)
        {
            disciplineRepository = FakeDisciplineRepository();
            disciplineRepository.GetDisciplineSummaryForOtherSystem(Arg.Any<GetDisciplineSummaryForOtherSystemCriteria>()).Returns(result ?? new List<DisciplineSummaryForOtherSystem>());
            disciplineService = GetDisciplineService(disciplineRepository);
        }

        #endregion Helpers
    }
}