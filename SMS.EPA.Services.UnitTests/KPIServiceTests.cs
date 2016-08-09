namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
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
        public void GetKPI_WhenCalled_ReturnListOfKPIModel()
        {
            // Arrange
            var criteria = new GetKPICriteria();
            kpiRepository = FakeKPIRepository();
            kpiService = GetKPIService(kpiRepository);

            var expected = new List<KPIModel>();
            kpiRepository.GetKPI(Arg.Any<GetKPICriteria>()).Returns(expected);

            // Act
            var actual = kpiService.GetKPI(criteria);

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<KPIModel>), actual);
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
    }
}