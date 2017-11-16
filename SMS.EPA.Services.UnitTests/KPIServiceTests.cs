namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using Criteria;
    using Implement;
    using Model;
    using NSubstitute;
    using NUnit.Framework;
    using Repositories;

    [TestFixture]
    public class KPIServiceTests
    {
        private KPIService _kpiService;
        private IKPIRepository _kpiRepository;

        #region GetKPI

        [Test]
        public void GetKPI_AnyArg_ReturnInstanceOfListOfKPIModel()
        {
            // Arrange
            ArrangeGetKPI();

            // Act
            var actual = _kpiService.GetKPI(new GetKPICriteria());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<KPIModel>), actual);
        }

        [Test]
        public void GetKPI_ValidArg_ReturnExpectedListOfKPIModel()
        {
            // Arrange
            var criteria = new GetKPICriteria();
            IEnumerable<KPIModel> result = GetListOfKPIForTest();
            ArrangeGetKPI(result);

            // Act
            IEnumerable<KPIModel> actual = _kpiService.GetKPI(criteria);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(result, actual);
        }

        [Test]
        public void GetKPI_ValidArg_KPIIncludeKPIGroup()
        {
            // Arrange
            var criteria = new GetKPICriteria();
            IEnumerable<KPIModel> result = GetListOfKPIForTest();
            ArrangeGetKPI(result);

            // Act
            IEnumerable<KPIModel> actual = _kpiService.GetKPI(criteria);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Where(x => !x.KPIGroups.Any()).Count());
        }

        #endregion GetKPI

        #region Helpers

        private IKPIRepository FakeKPIRepository()
        {
            var repository = Substitute.For<IKPIRepository>();

            return repository;
        }

        private KPIService GetKPIService(IKPIRepository kpiRepository = null)
        {
            var service = new KPIService(kpiRepository);

            return service;
        }

        private IEnumerable<KPIModel> GetListOfKPIForTest()
        {
            var kpis = new List<KPIModel>
            {
                new KPIModel
                {
                    KPIId = 1,
                    Name = "test 1",
                    KPIGroups = new List<KPIGroupModel>
                    {
                        new KPIGroupModel
                        {
                            KPIGroupId = 1,
                            Name = "group test 1"
                        }
                    }
                },
                new KPIModel
                {
                    KPIId = 2,
                    Name = "test 2",
                    KPIGroups = new List<KPIGroupModel>
                    {
                        new KPIGroupModel
                        {
                            KPIGroupId = 1,
                            Name = "group test 1"
                        }
                    }
                }
            };

            return kpis;
        }

        private void ArrangeGetKPI(IEnumerable<KPIModel> result = null)
        {
            _kpiRepository = FakeKPIRepository();
            _kpiRepository.GetKPI(Arg.Any<GetKPICriteria>()).Returns(result ?? new List<KPIModel>());
            _kpiService = GetKPIService(_kpiRepository);
        }

        #endregion Helpers
    }
}