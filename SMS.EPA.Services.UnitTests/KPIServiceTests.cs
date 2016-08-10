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

        [Test]
        public void CreateKPI_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            var service = Substitute.For<KPIService>();
            baseRepository = Substitute.For<IBaseRepository>();
            service.Repository = baseRepository;

            var expected = new CommandResult();
            service.Repository.Insert(Arg.Is<CreateKPICriteria>(criteria => criteria != null)).Returns(expected);

            // Act
            var actual = service.CreateKPI(new CreateKPICriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateKPI_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            var service = Substitute.For<KPIService>();
            baseRepository = Substitute.For<IBaseRepository>();
            service.Repository = baseRepository;

            var expected = new CommandResult();
            service.Repository.Update(Arg.Is<UpdateKPICriteria>(criteria => criteria != null)).Returns(expected);

            // Act
            var actual = service.UpdateKPI(new UpdateKPICriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateStatusKPI_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            var service = Substitute.For<KPIService>();
            baseRepository = Substitute.For<IBaseRepository>();
            service.Repository = baseRepository;

            var expected = new CommandResult();
            service.Repository.Update(Arg.Is<UpdateStatusKPICriteria>(criteria => criteria != null)).Returns(expected);

            // Act
            var actual = service.UpdateStatusKPI(new UpdateStatusKPICriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateRequiredEvaluateKPI_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            var service = Substitute.For<KPIService>();
            baseRepository = Substitute.For<IBaseRepository>();
            service.Repository = baseRepository;

            var expected = new CommandResult();
            service.Repository.Update(Arg.Is<UpdateRequiredEvaluateKPICriteria>(criteria => criteria != null)).Returns(expected);

            // Act
            var actual = service.UpdateRequiredEvaluateKPI(new UpdateRequiredEvaluateKPICriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void DeleteKPI_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            var service = Substitute.For<KPIService>();
            baseRepository = Substitute.For<IBaseRepository>();
            service.Repository = baseRepository;

            var expected = new CommandResult();
            service.Repository.Delete(Arg.Is<DeleteKPICriteria>(criteria => criteria != null)).Returns(expected);

            // Act
            var actual = service.DeleteKPI(new DeleteKPICriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        private IKPIRepository FakeKPIRepository()
        {
            IKPIRepository repository = Substitute.For<IKPIRepository>();

            return repository;
        }

        private IKPIService GetKPIService(IKPIRepository kpiRepository)
        {
            var service = new KPIService(kpiRepository);

            return service;
        }
    }
}