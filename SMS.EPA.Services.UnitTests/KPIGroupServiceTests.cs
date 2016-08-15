namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Repositories;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Implement;

    [TestFixture]
    public class KPIGroupServiceTests
    {
        private IBaseRepository baseRepository;

        [Test]
        public void GetKPIGroup_WhenCalled_ReturnsListOfKPIModel()
        {
            // Arrange
            var service = Substitute.For<KPIGroupService>();
            var expected = new List<KPIGroupModel>();
            service.When(x => x.GetData<KPIGroupModel>(Arg.Any<GetKPIGroupCriteria>())).DoNotCallBase();
            service.GetData<KPIGroupModel>(Arg.Any<GetKPIGroupCriteria>()).Returns(expected);

            // Act
            var actual = service.GetKPIGroup(new GetKPIGroupCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<KPIGroupModel>), actual);
        }

        [Test]
        public void GetKPIGroupById_WhenCalled_ReturnsKPIModel()
        {
            // Arrange
            var service = Substitute.For<KPIGroupService>();
            var expected = new KPIGroupModel();
            service.When(x => x.GetDataEntity<KPIGroupModel>(Arg.Any<GetKPIGroupByIdCriteria>())).DoNotCallBase();
            service.GetDataEntity<KPIGroupModel>(Arg.Any<GetKPIGroupByIdCriteria>()).Returns(expected);

            // Act
            var actual = service.GetKPIGroupById(new GetKPIGroupByIdCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(KPIGroupModel), actual);
        }

        [Test]
        public void CreateKPIGroup_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            baseRepository = Substitute.For<IBaseRepository>();
            var service = GetKPIGroupService(baseRepository);
            var expected = new CommandResult();
            service.Repository.Insert(Arg.Is<CreateKPIGroupCriteria>(criteria => criteria != null)).Returns(expected);

            // Act
            var actual = service.CreateKPIGroup(new CreateKPIGroupCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateKPIGroup_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            baseRepository = Substitute.For<IBaseRepository>();
            var service = GetKPIGroupService(baseRepository);
            var expected = new CommandResult();
            service.Repository.Update(Arg.Is<UpdateKPIGroupCriteria>(criteria => criteria != null)).Returns(expected);

            // Act
            var actual = service.UpdateKPIGroup(new UpdateKPIGroupCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateStatusKPIGroup_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            baseRepository = Substitute.For<IBaseRepository>();
            var service = GetKPIGroupService(baseRepository);
            var expected = new CommandResult();
            service.Repository.Update(Arg.Is<UpdateStatusKPIGroupCriteria>(criteria => criteria != null)).Returns(expected);

            // Act
            var actual = service.UpdateStatusKPIGroup(new UpdateStatusKPIGroupCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void DeleteKPIGroup_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            baseRepository = Substitute.For<IBaseRepository>();
            var service = GetKPIGroupService(baseRepository);
            var expected = new CommandResult();
            service.Repository.Delete(Arg.Is<DeleteKPIGroupCriteria>(criteria => criteria != null)).Returns(expected);

            // Act
            var actual = service.DeleteKPIGroup(new DeleteKPIGroupCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        private KPIGroupService GetKPIGroupService(IBaseRepository baseRepository = null)
        {
            var service = new KPIGroupService();
            service.Repository = baseRepository;

            return service;
        }
    }
}