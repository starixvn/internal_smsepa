namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using Criteria;
    using Implement;
    using Model.Discipline;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Services;

    [TestFixture]
    public class DisciplineServiceTests
    {
        private IBaseService baseService;

        #region GetDisciplineSummary

        [Test]
        public void GetDisciplineSummary_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var expected = new List<DisciplineSummaryModel>
            {
                new DisciplineSummaryModel
                {
                    EmployeeId = 1,
                    LateIn = 2.5M
                }
            };
            var criteria = new GetDisciplineSummaryCriteria();
            var service = GetDisciplineService();
            baseService.GetData<DisciplineSummaryModel>(Arg.Any<GetDisciplineSummaryCriteria>()).Returns(expected);

            // Act
            var actual = service.GetDisciplineSummary(criteria);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion GetDisciplineSummary

        #region GetDisciplineSummaryForOtherSystem

        [Test]
        public void GetDisciplineSummaryForOtherSystem_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var expected = new List<DisciplineSummaryForOtherSystem>
            {
                new DisciplineSummaryForOtherSystem
                {
                    EmployeeId = 1,
                    LateIn = 2.5M
                }
            };
            var criteria = new GetDisciplineSummaryForOtherSystemCriteria();
            var service = GetDisciplineService();
            baseService.GetData<DisciplineSummaryForOtherSystem>(Arg.Any<GetDisciplineSummaryForOtherSystemCriteria>()).Returns(expected);

            // Act
            var actual = service.GetDisciplineSummaryForOtherSystem(criteria);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion GetDisciplineSummaryForOtherSystem

        #region Helpers

        private DisciplineService GetDisciplineService()
        {
            baseService = Substitute.For<IBaseService>();
            return new DisciplineService(baseService);
        }

        #endregion Helpers
    }
}