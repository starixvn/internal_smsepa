namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.EPAHistory;
    using SMS.EPA.Model.EPAHistory;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    [TestFixture]
    public class EPAHistoryServiceTests
    {
        private IEPAHistoryRepository epaHistoryRepository;
        private IEPAHistoryService epaHistoryService;
        private ICryptographyService cryptographyService;

        #region GetEPAHistory

        [Test]
        public void GetEPAHistory_AnyArg_ReturnInstanceOfListOfEPAHistoryModel()
        {
            // Arrange
            epaHistoryService = Substitute.For<IEPAHistoryService>();

            // Act
            var actual = epaHistoryService.GetEPAHistory(Arg.Any<GetEPAHistoryCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IList<EPAHistoryModel>), actual);
        }

        [Test]
        public void GetEPAHistory_VaidArg_ReturnExpectedEPAHistory()
        {
            // Arrange
            var criteria = new GetEPAHistoryCriteria()
            {
                EvaluatorId = 1
            };
            var expectedEPAs = new List<EPAHistoryModel>();
            expectedEPAs.Add(new EPAHistoryModel());
            ArrangeGetEPAHistory(expectedEPAs);

            // Act
            var actual = epaHistoryService.GetEPAHistory(criteria);

            // Assert
            Assert.AreEqual(expectedEPAs, actual);
        }

        [Test]
        public void GetEPAHistory_InVaidArg_ReturnDefaultEPAHistory()
        {
            // Arrange
            GetEPAHistoryCriteria criteria = new GetEPAHistoryCriteria();
            var expectedEPAs = new List<EPAHistoryModel>();
            expectedEPAs.Add(new EPAHistoryModel());
            ArrangeGetEPAHistory(expectedEPAs);

            // Act
            var actual = epaHistoryService.GetEPAHistory(criteria);

            // Assert
            Assert.AreNotEqual(expectedEPAs, actual);
        }

        #endregion GetEPAHistory

        #region GetMonthlyEPAHistory

        [Test]
        public void GetMonthlyEPAHistory_AnyArg_ReturnInstanceOfListOfEPAHistoryModel()
        {
            // Arrange
            epaHistoryService = Substitute.For<IEPAHistoryService>();

            // Act
            var actual = epaHistoryService.GetMonthlyEPAHistory(Arg.Any<GetMonthlyEPAHistoryCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IList<EPAHistoryModel>), actual);
        }

        [Test]
        public void GetMonthlyEPAHistory_VaidArg_ReturnExpectedEPAHistory()
        {
            // Arrange
            var criteria = new GetMonthlyEPAHistoryCriteria()
            {
                EvaluatorId = 1
            };
            var expectedEPAs = new List<EPAHistoryModel>();
            expectedEPAs.Add(new EPAHistoryModel());
            ArrangeGetMonthlyEPAHistory(expectedEPAs);

            // Act
            var actual = epaHistoryService.GetMonthlyEPAHistory(criteria);

            // Assert
            Assert.AreEqual(expectedEPAs, actual);
        }

        [Test]
        public void GetMonthlyEPAHistory_InVaidArg_ReturnDefaultEPAHistory()
        {
            // Arrange
            var criteria = new GetMonthlyEPAHistoryCriteria();
            var expectedEPAs = new List<EPAHistoryModel>();
            expectedEPAs.Add(new EPAHistoryModel());
            ArrangeGetMonthlyEPAHistory(expectedEPAs);

            // Act
            var actual = epaHistoryService.GetMonthlyEPAHistory(criteria);

            // Assert
            Assert.AreNotEqual(expectedEPAs, actual);
        }

        #endregion GetMonthlyEPAHistory

        #region GetAllPeriodIncludeImport

        [Test]
        public void GetAllPeriodIncludeImport_AnyArg_ReturnInstanceOfListOfPeriodModel()
        {
            // Arrange
            epaHistoryService = Substitute.For<IEPAHistoryService>();

            // Act
            var actual = epaHistoryService.GetAllPeriodIncludeImport(Arg.Any<GetAllPeriodIncludeImportCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IList<PeriodModel>), actual);
        }

        [Test]
        public void GetAllPeriodIncludeImport_VaidArg_ReturnExpectedEPAHistory()
        {
            // Arrange
            var criteria = new GetAllPeriodIncludeImportCriteria();
            var expectedPeriods = new List<PeriodModel>();
            expectedPeriods.Add(new PeriodModel());
            var epaHistoryService = ArrangeGetAllPeriodIncludeImport(expectedPeriods);

            // Act
            var actual = epaHistoryService.GetAllPeriodIncludeImport(criteria);

            // Assert
            Assert.AreEqual(expectedPeriods, actual);
        }

        #endregion GetAllPeriodIncludeImport

        private IEPAHistoryRepository FakeEPAHistoryRepository()
        {
            var repository = Substitute.For<IEPAHistoryRepository>();

            return repository;
        }

        private ICryptographyService FakeCryptographyService()
        {
            var repository = Substitute.For<ICryptographyService>();

            return repository;
        }

        private void ArrangeGetEPAHistory(IList<EPAHistoryModel> expectedEPAs = null)
        {
            epaHistoryRepository = FakeEPAHistoryRepository();
            cryptographyService = FakeCryptographyService();
            epaHistoryRepository.GetEPAHistory(Arg.Any<GetEPAHistoryCriteria>()).Returns(expectedEPAs ?? new List<EPAHistoryModel>());
            epaHistoryService = new EPAHistoryService(epaHistoryRepository, cryptographyService);
        }

        private void ArrangeGetMonthlyEPAHistory(IList<EPAHistoryModel> expectedEPAs = null)
        {
            epaHistoryRepository = FakeEPAHistoryRepository();
            cryptographyService = FakeCryptographyService();
            epaHistoryRepository.GetMonthlyEPAHistory(Arg.Any<GetMonthlyEPAHistoryCriteria>()).Returns(expectedEPAs ?? new List<EPAHistoryModel>());
            epaHistoryService = new EPAHistoryService(epaHistoryRepository, cryptographyService);
        }

        private EPAHistoryService ArrangeGetAllPeriodIncludeImport(IList<PeriodModel> periods = null)
        {
            var epaHistoryService = Substitute.For<EPAHistoryService>();
            epaHistoryService.GetData<PeriodModel>(Arg.Any<GetAllPeriodIncludeImportCriteria>()).Returns(periods ?? new List<PeriodModel>());
            return epaHistoryService;
        }
    }
}