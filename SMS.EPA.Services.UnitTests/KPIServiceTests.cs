namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Repositories;
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
        private IBaseRepository baseRepository;

        [Test]
        public void GetKPI_WhenCalled_ReturnListOfKPIModel()
        {
            // Arrange
            kpiRepository = FakeKPIRepository();
            var expected = new List<KPIModel>();
            kpiRepository.GetKPI(Arg.Any<GetKPICriteria>()).Returns(expected);
            kpiService = GetKPIService(kpiRepository);

            // Act
            var actual = kpiService.GetKPI(new GetKPICriteria());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<KPIModel>), actual);
        }

        [Test]
        public void CreateKPI_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            baseRepository = Substitute.For<IBaseRepository>();
            var service = GetKPIService(null, baseRepository);
            var expected = new CommandResult();
            service.Repository.Insert(Arg.Is<CreateKPICriteria>(criteria => criteria != null)).Returns(expected);
            kpiService = FakeKPIService();

            // Act
            var actual = kpiService.CreateKPI(new CreateKPICriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateKPI_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            baseRepository = Substitute.For<IBaseRepository>();
            var service = GetKPIService(null, baseRepository);
            var expected = new CommandResult();
            service.Repository.Update(Arg.Is<UpdateKPICriteria>(criteria => criteria != null)).Returns(expected);
            kpiService = FakeKPIService();

            // Act
            var actual = kpiService.UpdateKPI(new UpdateKPICriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateStatusKPI_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            baseRepository = Substitute.For<IBaseRepository>();
            var service = GetKPIService(null, baseRepository);
            var expected = new CommandResult();
            service.Repository.Update(Arg.Is<UpdateStatusKPICriteria>(criteria => criteria != null)).Returns(expected);
            kpiService = FakeKPIService();

            // Act
            var actual = kpiService.UpdateStatusKPI(new UpdateStatusKPICriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateRequiredEvaluateKPI_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            baseRepository = Substitute.For<IBaseRepository>();
            var service = GetKPIService(null, baseRepository);
            var expected = new CommandResult();
            service.Repository.Update(Arg.Is<UpdateRequiredEvaluateKPICriteria>(criteria => criteria != null)).Returns(expected);
            kpiService = FakeKPIService();

            // Act
            var actual = kpiService.UpdateRequiredEvaluateKPI(new UpdateRequiredEvaluateKPICriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void DeleteKPI_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            baseRepository = Substitute.For<IBaseRepository>();
            var service = GetKPIService(null, baseRepository);
            var expected = new CommandResult();
            service.Repository.Delete(Arg.Is<DeleteKPICriteria>(criteria => criteria != null)).Returns(expected);
            kpiService = FakeKPIService();

            // Act
            var actual = kpiService.DeleteKPI(new DeleteKPICriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        private IKPIRepository FakeKPIRepository()
        {
            var repository = Substitute.For<IKPIRepository>();

            return repository;
        }

        private IKPIService FakeKPIService()
        {
            var service = Substitute.For<KPIService>();

            return service;
        }

        private KPIService GetKPIService(IKPIRepository kpiRepository = null, IBaseRepository baseRepository = null)
        {
            var service = new KPIService(kpiRepository);
            service.Repository = baseRepository;

            return service;
        }
    }
}