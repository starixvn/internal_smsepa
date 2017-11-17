namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using Core.Services;
    using Criteria.EPAHistory;
    using Implement;
    using Interfaces;
    using Model.EPAHistory;
    using NSubstitute;
    using NUnit.Framework;
    using Repositories;

    [TestFixture]
    public class EPAHistoryServiceTests
    {
        private IEPAHistoryRepository _epaHistoryRepository;
        private IEPAHistoryService _epaHistoryService;
        private ICryptographyService _cryptographyService;

        #region GetEPAHistory

        [Test]
        public void GetEPAHistory_AnyArg_ReturnInstanceOfListOfEPAHistoryModel()
        {
            // Arrange
            _epaHistoryService = Substitute.For<IEPAHistoryService>();

            // Act
            var actual = _epaHistoryService.GetEPAHistory(Arg.Any<GetEPAHistoryCriteria>());

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
            var expectedEPAs = new List<EPAHistoryModel> { new EPAHistoryModel() };
            ArrangeGetEPAHistory(expectedEPAs);

            // Act
            var actual = _epaHistoryService.GetEPAHistory(criteria);

            // Assert
            Assert.AreEqual(expectedEPAs, actual);
        }

        [Test]
        public void GetEPAHistory_InVaidArg_ReturnDefaultEPAHistory()
        {
            // Arrange
            GetEPAHistoryCriteria criteria = new GetEPAHistoryCriteria();
            var expectedEPAs = new List<EPAHistoryModel> { new EPAHistoryModel() };
            ArrangeGetEPAHistory(expectedEPAs);

            // Act
            var actual = _epaHistoryService.GetEPAHistory(criteria);

            // Assert
            Assert.AreNotEqual(expectedEPAs, actual);
        }

        #endregion GetEPAHistory

        #region GetMonthlyEPAHistory

        [Test]
        public void GetMonthlyEPAHistory_AnyArg_ReturnInstanceOfListOfEPAHistoryModel()
        {
            // Arrange
            _epaHistoryService = Substitute.For<IEPAHistoryService>();

            // Act
            var actual = _epaHistoryService.GetMonthlyEPAHistory(Arg.Any<GetMonthlyEPAHistoryCriteria>());

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
            var expectedEPAs = new List<EPAHistoryModel> { new EPAHistoryModel() };
            ArrangeGetMonthlyEPAHistory(expectedEPAs);

            // Act
            var actual = _epaHistoryService.GetMonthlyEPAHistory(criteria);

            // Assert
            Assert.AreEqual(expectedEPAs, actual);
        }

        [Test]
        public void GetMonthlyEPAHistory_InVaidArg_ReturnDefaultEPAHistory()
        {
            // Arrange
            var criteria = new GetMonthlyEPAHistoryCriteria();
            var expectedEPAs = new List<EPAHistoryModel> { new EPAHistoryModel() };
            ArrangeGetMonthlyEPAHistory(expectedEPAs);

            // Act
            var actual = _epaHistoryService.GetMonthlyEPAHistory(criteria);

            // Assert
            Assert.AreNotEqual(expectedEPAs, actual);
        }

        #endregion GetMonthlyEPAHistory

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
            _epaHistoryRepository = FakeEPAHistoryRepository();
            _cryptographyService = FakeCryptographyService();
            _epaHistoryRepository.GetEPAHistory(Arg.Any<GetEPAHistoryCriteria>()).Returns(expectedEPAs ?? new List<EPAHistoryModel>());
            _epaHistoryService = new EPAHistoryService(_epaHistoryRepository, _cryptographyService);
        }

        private void ArrangeGetMonthlyEPAHistory(IList<EPAHistoryModel> expectedEPAs = null)
        {
            _epaHistoryRepository = FakeEPAHistoryRepository();
            _cryptographyService = FakeCryptographyService();
            _epaHistoryRepository.GetMonthlyEPAHistory(Arg.Any<GetMonthlyEPAHistoryCriteria>()).Returns(expectedEPAs ?? new List<EPAHistoryModel>());
            _epaHistoryService = new EPAHistoryService(_epaHistoryRepository, _cryptographyService);
        }
    }
}