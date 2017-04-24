namespace SMS.EPA.Services.UnitTests
{
    using Core.Model;
    using Core.Repositories;
    using Criteria;
    using Implement;
    using Model;
    using NSubstitute;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class KPIGroupServiceTests
    {
        private KPIGroupService _kpiGroupService;
        private IBaseRepository _baseRepository;

        #region GetKPIGroup

        [Test]
        public void GetKPIGroup_AnyArg_ReturnInstanceOfListOfKPIGroupModel()
        {
            // Arrange
            ArrangeGetKPIGroup();

            // Act
            var actual = _kpiGroupService.GetKPIGroup(new GetKPIGroupCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<KPIGroupModel>), actual);
        }

        [Test]
        public void GetKPIGroup_ValidArg_ReturnListOfKPIGroupModel()
        {
            // Arrange
            var criteria = new GetKPIGroupCriteria();
            IEnumerable<KPIGroupModel> result = GetListOfKPIGroupForTest();
            ArrangeGetKPIGroup(result);

            // Act
            var actual = _kpiGroupService.GetKPIGroup(criteria);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(result, actual);
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
            var actual = _kpiGroupService.GetKPIGroupById(kpiGroupId);

            // Assert
            Assert.IsInstanceOf(typeof(KPIGroupModel), actual);
        }

        [Test]
        public void GetKPIGroupById_ValidArg_ReturnKPIGroupModel()
        {
            // Arrange
            int kpiGroupId = 1;
            var result = GetListOfKPIGroupForTest().First();
            ArrangeGetKPIGroupById(result);

            // Act
            var actual = _kpiGroupService.GetKPIGroupById(kpiGroupId);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(result, actual);
            Assert.AreEqual(kpiGroupId, actual.KPIGroupId);
        }

        #endregion GetKPIGroupById

        #region CreateKPIGroup

        [Test]
        public void CreateKPIGroup_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeCreateKPIGroup();

            // Act
            var actual = _kpiGroupService.CreateKPIGroup(new CreateKPIGroupCriteria());

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
            _kpiGroupService.CreateKPIGroup(criteria);

            // Assert
            _kpiGroupService.Repository.Received().Insert(Arg.Is<CreateKPIGroupCriteria>(x => x.IsValid()));
        }

        #endregion CreateKPIGroup

        #region UpdateKPIGroup

        [Test]
        public void UpdateKPIGroup_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeUpdateKPIGroup();

            // Act
            var actual = _kpiGroupService.UpdateKPIGroup(new UpdateKPIGroupCriteria());

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
            _kpiGroupService.UpdateKPIGroup(criteria);

            // Assert
            _kpiGroupService.Repository.Received().Update(Arg.Is<UpdateKPIGroupCriteria>(x => x.IsValid()));
        }

        #endregion UpdateKPIGroup

        #region UpdateStatusKPIGroup

        [Test]
        public void UpdateStatusKPIGroup_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeUpdateStatusKPIGroup();

            // Act
            var actual = _kpiGroupService.UpdateStatusKPIGroup(new UpdateStatusKPIGroupCriteria());

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
            _kpiGroupService.UpdateStatusKPIGroup(criteria);

            // Assert
            _kpiGroupService.Repository.Received().Update(Arg.Is<UpdateStatusKPIGroupCriteria>(x => x.IsValid()));
        }

        #endregion UpdateStatusKPIGroup

        #region DeleteKPIGroup

        [Test]
        public void DeleteKPIGroup_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeDeleteKPIGroup();

            // Act
            var actual = _kpiGroupService.DeleteKPIGroup(new DeleteKPIGroupCriteria());

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
            _kpiGroupService.DeleteKPIGroup(criteria);

            // Assert
            _kpiGroupService.Repository.Received().Delete(Arg.Is<DeleteKPIGroupCriteria>(x => x.IsValid()));
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

        private IEnumerable<KPIGroupModel> GetListOfKPIGroupForTest()
        {
            var kpiGroups = new List<KPIGroupModel>
            {
                new KPIGroupModel
                {
                    KPIGroupId = 1,
                    Name = "group test 1"
                },
                new KPIGroupModel
                {
                    KPIGroupId = 2,
                    Name = "group test 2"
                }
            };

            return kpiGroups;
        }

        private void ArrangeGetKPIGroup(IEnumerable<KPIGroupModel> result = null)
        {
            _kpiGroupService = Substitute.For<KPIGroupService>();
            _kpiGroupService.When(x => x.GetData<KPIGroupModel>(Arg.Any<GetKPIGroupCriteria>())).DoNotCallBase();
            _kpiGroupService.GetData<KPIGroupModel>(Arg.Any<GetKPIGroupCriteria>()).Returns(result ?? new List<KPIGroupModel>());
        }

        private void ArrangeGetKPIGroupById(KPIGroupModel result = null)
        {
            _kpiGroupService = Substitute.For<KPIGroupService>();
            _kpiGroupService.When(x => x.GetDataEntity<KPIGroupModel>(Arg.Any<GetKPIGroupByIdCriteria>())).DoNotCallBase();
            _kpiGroupService.GetDataEntity<KPIGroupModel>(Arg.Any<GetKPIGroupByIdCriteria>()).Returns(result ?? new KPIGroupModel());
        }

        private void ArrangeCreateKPIGroup(CommandResult result = null)
        {
            _baseRepository = FakeBaseRepository();
            _kpiGroupService = GetKPIGroupService(_baseRepository);
            _kpiGroupService.Repository.Insert(Arg.Any<CreateKPIGroupCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeUpdateKPIGroup(CommandResult result = null)
        {
            _baseRepository = FakeBaseRepository();
            _kpiGroupService = GetKPIGroupService(_baseRepository);
            _kpiGroupService.Repository.Update(Arg.Any<UpdateKPIGroupCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeUpdateStatusKPIGroup(CommandResult result = null)
        {
            _baseRepository = FakeBaseRepository();
            _kpiGroupService = GetKPIGroupService(_baseRepository);
            _kpiGroupService.Repository.Update(Arg.Any<UpdateStatusKPIGroupCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeDeleteKPIGroup(CommandResult result = null)
        {
            _baseRepository = FakeBaseRepository();
            _kpiGroupService = GetKPIGroupService(_baseRepository);
            _kpiGroupService.Repository.Delete(Arg.Any<DeleteKPIGroupCriteria>()).Returns(result ?? new CommandResult());
        }

        #endregion Helpers
    }
}