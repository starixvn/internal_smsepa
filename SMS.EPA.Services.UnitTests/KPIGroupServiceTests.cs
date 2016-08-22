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
        private KPIGroupService kpiGroupService;
        private IBaseRepository baseRepository;

        #region GetKPIGroup

        [Test]
        public void GetKPIGroup_AnyArg_ReturnInstanceOfListOfKPIModel()
        {
            // Arrange
            ArrangeGetKPIGroup();

            // Act
            var actual = kpiGroupService.GetKPIGroup(new GetKPIGroupCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<KPIGroupModel>), actual);
        }

        [Test]
        public void GetKPIGroup_ValidArg_ExpectedToExecuteGetData()
        {
            // Arrange
            var criteria = new GetKPIGroupCriteria();
            ArrangeGetKPIGroup();

            // Act
            kpiGroupService.GetKPIGroup(criteria);

            // Assert
            kpiGroupService.Received().GetData<KPIGroupModel>(Arg.Is<GetKPIGroupCriteria>(x => x.IsValid()));
        }

        #endregion GetKPIGroup

        #region GetKPIGroupById

        [Test]
        public void GetKPIGroupById_AnyArg_ReturnInstanceOfKPIModel()
        {
            // Arrange
            int kpiGroupId = 1;
            ArrangeGetKPIGroupById();

            // Act
            var actual = kpiGroupService.GetKPIGroupById(kpiGroupId);

            // Assert
            Assert.IsInstanceOf(typeof(KPIGroupModel), actual);
        }

        [Test]
        public void GetKPIGroupById_ValidArg_ExpectedToExecuteGetDataEntity()
        {
            // Arrange
            int kpiGroupId = 1;
            ArrangeGetKPIGroupById();

            // Act
            kpiGroupService.GetKPIGroupById(kpiGroupId);

            // Assert
            kpiGroupService.Received().GetDataEntity<KPIGroupModel>(Arg.Is<GetKPIGroupByIdCriteria>(x => x.IsValid()));
        }

        #endregion GetKPIGroupById

        #region CreateKPIGroup

        [Test]
        public void CreateKPIGroup_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeCreateKPIGroup();

            // Act
            var actual = kpiGroupService.CreateKPIGroup(new CreateKPIGroupCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void CreateKPIGroup_ValidArg_ExpectedToExecuteInsert()
        {
            // Arrange
            var criteria = new CreateKPIGroupCriteria
            {
                Name = "test",
                CreatedBy = 1
            };
            ArrangeCreateKPIGroup();

            // Act
            kpiGroupService.CreateKPIGroup(criteria);

            // Assert
            kpiGroupService.Repository.Received().Insert(Arg.Is<CreateKPIGroupCriteria>(x => x.IsValid()));
        }

        #endregion CreateKPIGroup

        #region UpdateKPIGroup

        [Test]
        public void UpdateKPIGroup_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeUpdateKPIGroup();

            // Act
            var actual = kpiGroupService.UpdateKPIGroup(new UpdateKPIGroupCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateKPIGroup_ValidArg_ExpectedToExecuteUpdate()
        {
            // Arrange
            var criteria = new UpdateKPIGroupCriteria
            {
                KPIGroupId = 1,
                Name = "test",
                ModifiedBy = 1
            };
            ArrangeUpdateKPIGroup();

            // Act
            kpiGroupService.UpdateKPIGroup(criteria);

            // Assert
            kpiGroupService.Repository.Received().Update(Arg.Is<UpdateKPIGroupCriteria>(x => x.IsValid()));
        }

        #endregion UpdateKPIGroup

        #region UpdateStatusKPIGroup

        [Test]
        public void UpdateStatusKPIGroup_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeUpdateStatusKPIGroup();

            // Act
            var actual = kpiGroupService.UpdateStatusKPIGroup(new UpdateStatusKPIGroupCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateStatusKPIGroup_ValidArg_ExpectedToExecuteUpdate()
        {
            // Arrange
            var criteria = new UpdateStatusKPIGroupCriteria
            {
                KPIGroupId = 1,
                ModifiedBy = 1
            };
            ArrangeUpdateStatusKPIGroup();

            // Act
            kpiGroupService.UpdateStatusKPIGroup(criteria);

            // Assert
            kpiGroupService.Repository.Received().Update(Arg.Is<UpdateStatusKPIGroupCriteria>(x => x.IsValid()));
        }

        #endregion UpdateStatusKPIGroup

        #region DeleteKPIGroup

        [Test]
        public void DeleteKPIGroup_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeDeleteKPIGroup();

            // Act
            var actual = kpiGroupService.DeleteKPIGroup(new DeleteKPIGroupCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void DeleteKPIGroup_AnyArg_ExpectedToExecuteDelete()
        {
            // Arrange
            var criteria = new DeleteKPIGroupCriteria
            {
                KPIGroupId = 1,
                ModifiedBy = 1
            };
            ArrangeDeleteKPIGroup();

            // Act
            kpiGroupService.DeleteKPIGroup(criteria);

            // Assert
            kpiGroupService.Repository.Received().Delete(Arg.Is<DeleteKPIGroupCriteria>(x => x.IsValid()));
        }

        #endregion DeleteKPIGroup

        #region Helpers

        private IBaseRepository FakeBaseRepository()
        {
            var repository = Substitute.For<IBaseRepository>();

            return repository;
        }

        private KPIGroupService GetKPIGroupService(IBaseRepository baseRepository = null)
        {
            var service = new KPIGroupService();
            service.Repository = baseRepository;

            return service;
        }

        private void ArrangeGetKPIGroup(IEnumerable<KPIGroupModel> result = null)
        {
            kpiGroupService = Substitute.For<KPIGroupService>();
            kpiGroupService.When(x => x.GetData<KPIGroupModel>(Arg.Any<GetKPIGroupCriteria>())).DoNotCallBase();
            kpiGroupService.GetData<KPIGroupModel>(Arg.Any<GetKPIGroupCriteria>()).Returns(result ?? new List<KPIGroupModel>());
        }

        private void ArrangeGetKPIGroupById(KPIGroupModel result = null)
        {
            kpiGroupService = Substitute.For<KPIGroupService>();
            kpiGroupService.When(x => x.GetDataEntity<KPIGroupModel>(Arg.Any<GetKPIGroupByIdCriteria>())).DoNotCallBase();
            kpiGroupService.GetDataEntity<KPIGroupModel>(Arg.Any<GetKPIGroupByIdCriteria>()).Returns(result ?? new KPIGroupModel());
        }

        private void ArrangeCreateKPIGroup(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            kpiGroupService = GetKPIGroupService(baseRepository);
            kpiGroupService.Repository.Insert(Arg.Any<CreateKPIGroupCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeUpdateKPIGroup(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            kpiGroupService = GetKPIGroupService(baseRepository);
            kpiGroupService.Repository.Update(Arg.Any<UpdateKPIGroupCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeUpdateStatusKPIGroup(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            kpiGroupService = GetKPIGroupService(baseRepository);
            kpiGroupService.Repository.Update(Arg.Any<UpdateStatusKPIGroupCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeDeleteKPIGroup(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            kpiGroupService = GetKPIGroupService(baseRepository);
            kpiGroupService.Repository.Delete(Arg.Any<DeleteKPIGroupCriteria>()).Returns(result ?? new CommandResult());
        }

        #endregion Helpers
    }
}