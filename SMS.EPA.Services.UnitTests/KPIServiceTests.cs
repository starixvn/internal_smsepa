namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    [TestFixture]
    public class KPIServiceTests
    {
        private IKPIService kpiService;
        private IKPIRepository kpiRepository;

        [Test]
        public void GetKPI_AnyArg_ReturnInstanceOfListKPIModel()
        {
            var criteria = new GetKPICriteria();
            kpiRepository = FakeKPIRepository();
            kpiService = GetKPIService(kpiRepository);

            var expected = new List<KPIModel>();
            kpiRepository.GetKPI(Arg.Any<GetKPICriteria>()).Returns(expected);

            var actual = kpiService.GetKPI(criteria);
            Assert.IsInstanceOf(typeof(List<KPIModel>), actual);
        }

        [Test]
        public void GetKPI_AnyArg_ReturnListOfKPIModel()
        {
            var criteria = new GetKPICriteria();
            kpiRepository = FakeKPIRepository();
            kpiService = GetKPIService(kpiRepository);

            var kpis = GetKPIForTest();
            kpiRepository.GetKPI(Arg.Any<GetKPICriteria>()).Returns(kpis);

            var actual = kpiService.GetKPI(criteria).Count();
            Assert.AreEqual(2, actual);
        }

        private IKPIRepository FakeKPIRepository()
        {
            var repository = Substitute.For<IKPIRepository>();

            return repository;
        }

        private IKPIService GetKPIService(IKPIRepository kpiRepository)
        {
            var service = new KPIService(kpiRepository);

            return service;
        }

        private IEnumerable<KPIModel> GetKPIForTest()
        {
            return new List<KPIModel>
            {
                new KPIModel
                {
                    KPIId = 1,
                    Name = "kpi 1",
                    IsRequiredEvaluate = true,
                    IsActive = true,
                    KPIGroups = new List<KPIGroupModel>
                    {
                        new KPIGroupModel
                        {
                            KPIGroupId = 1,
                            Name = "kpi group 1"
                        }
                    }
                },
                new KPIModel
                {
                    KPIId = 2,
                    Name = "kpi 2",
                    IsRequiredEvaluate = false,
                    IsActive = false,
                    KPIGroups = new List<KPIGroupModel>
                    {
                        new KPIGroupModel
                        {
                            KPIGroupId = 1,
                            Name = "kpi group 1"
                        }
                    }
                }
            };
        }
    }
}