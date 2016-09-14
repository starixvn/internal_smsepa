namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Repositories;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Services.Implement;

    [TestFixture]
    public class TargetTemplateServiceTests
    {
        private TargetTemplateService targetTemplateService;
        private IBaseRepository baseRepository;

        #region GetTargetTemplateById

        [Test]
        public void GetTargetTemplateById_AnyArg_ReturnInstanceOfTargetTemplateModel()
        {
            // Arrange
            ArrangeGetTargetTemplateById();

            // Act
            var actual = targetTemplateService.GetTargetTemplateById(new GetTargetTemplateByIdCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(TargetTemplate), actual);
        }

        [Test]
        public void GetTargetTemplateById_ValidArg_ExpectedToExecuteGetDataEntity()
        {
            // Arrange
            var criteria = new GetTargetTemplateByIdCriteria
            {
                TargetTemplateId = 1
            };
            ArrangeGetTargetTemplateById();

            // Act
            targetTemplateService.GetTargetTemplateById(criteria);

            // Assert
            targetTemplateService.Received().GetDataEntity<TargetTemplate>(Arg.Is<GetTargetTemplateByIdCriteria>(x => x.IsValid()));
        }

        #endregion GetTargetTemplateById

        #region GetAllTargetTemplate

        [Test]
        public void GetAllTargetTemplate_AnyArg_ReturnInstanceOfListOfTargetTemplateModel()
        {
            // Arrange
            ArrangeGetAllTargetTemplate();

            // Act
            var actual = targetTemplateService.GetAllTargetTemplate(new GetAllTargetTemplateCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<TargetTemplate>), actual);
        }

        [Test]
        public void GetAllTargetTemplate_ValidArg_ExpectedToExecuteGetData()
        {
            // Arrange
            var criteria = new GetAllTargetTemplateCriteria();
            ArrangeGetAllTargetTemplate();

            // Act
            targetTemplateService.GetAllTargetTemplate(criteria);

            // Assert
            targetTemplateService.Received().GetData<TargetTemplate>(Arg.Is<GetAllTargetTemplateCriteria>(x => x.IsValid()));
        }

        #endregion GetAllTargetTemplate

        #region GetRatingType

        [Test]
        public void GetRatingType_AnyArg_ReturnInstanceOfListOfRatingType()
        {
            // Arrange
            ArrangeGetRatingType();

            // Act
            var actual = targetTemplateService.GetRatingType(new GetRatingTypeCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<RatingType>), actual);
        }

        [Test]
        public void GetRatingType_ValidArg_ExpectedToExecuteGetData()
        {
            // Arrange
            var criteria = new GetRatingTypeCriteria();
            ArrangeGetRatingType();

            // Act
            targetTemplateService.GetRatingType(criteria);

            // Assert
            targetTemplateService.Received().GetData<RatingType>(Arg.Is<GetRatingTypeCriteria>(x => x.IsValid()));
        }

        #endregion GetRatingType

        #region CreateTargetTemplate

        [Test]
        public void CreateTargetTemplate_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeCreateTargetTemplate();

            // Act
            var actual = targetTemplateService.CreateTargetTemplate(new CreateTargetTemplateCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void CreateTargetTemplate_ValidArg_ExpectedToExecuteInsert()
        {
            // Arrange
            var criteria = new CreateTargetTemplateCriteria
            {
                Name = "test",
                TargetTypeId = 1,
                ScoreRatingTypeId = 1,
                RatingTypeId = 1,
                CreatedBy = 1
            };
            ArrangeCreateTargetTemplate();

            // Act
            targetTemplateService.CreateTargetTemplate(criteria);

            // Assert
            targetTemplateService.Repository.Received().Insert(Arg.Is<CreateTargetTemplateCriteria>(x => x.IsValid()));
        }

        #endregion CreateTargetTemplate

        #region UpdateStatusTargetTemplate

        [Test]
        public void UpdateStatusTargetTemplate_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeUpdateStatusTargetTemplate();

            // Act
            var actual = targetTemplateService.UpdateStatusTargetTemplate(new UpdateStatusTargetTemplateCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateStatusTargetTemplate_ValidArg_ExpectedToExecuteUpdate()
        {
            // Arrange
            var criteria = new UpdateStatusTargetTemplateCriteria
            {
                TargetTemplateId = 1,
                ModifiedBy = 1
            };
            ArrangeUpdateStatusTargetTemplate();

            // Act
            targetTemplateService.UpdateStatusTargetTemplate(criteria);

            // Assert
            targetTemplateService.Repository.Received().Update(Arg.Is<UpdateStatusTargetTemplateCriteria>(x => x.IsValid()));
        }

        #endregion UpdateStatusTargetTemplate

        #region UpdateTargetTemplate

        [Test]
        public void UpdateTargetTemplate_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeUpdateTargetTemplate();

            // Act
            var actual = targetTemplateService.UpdateTargetTemplate(new UpdateTargetTemplateCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateTargetTemplate_ValidArg_ExpectedToExecuteUpdate()
        {
            // Arrange
            var criteria = new UpdateTargetTemplateCriteria
            {
                TargetTemplateId = 1,
                ModifiedBy = 1
            };
            ArrangeUpdateTargetTemplate();

            // Act
            targetTemplateService.UpdateTargetTemplate(criteria);

            // Assert
            targetTemplateService.Repository.Received().Update(Arg.Is<UpdateTargetTemplateCriteria>(x => x.IsValid()));
        }

        #endregion UpdateTargetTemplate

        #region UpdateTargetTemplateBasicInfo

        [Test]
        public void UpdateTargetTemplateBasicInfo_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeUpdateTargetTemplateBasicInfo();

            // Act
            var actual = targetTemplateService.UpdateTargetTemplateBasicInfo(new UpdateTargetTemplateBasicInfoCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateTargetTemplateBasicInfo_ValidArg_ExpectedToExecuteUpdate()
        {
            // Arrange
            var criteria = new UpdateTargetTemplateBasicInfoCriteria
            {
                TargetTemplateId = 1,
                Name = "test",
                TargetTypeId = 1,
                ScoreRatingTypeId = 1,
                RatingTypeId = 1,
                ModifiedBy = 1
            };
            ArrangeUpdateTargetTemplateBasicInfo();

            // Act
            targetTemplateService.UpdateTargetTemplateBasicInfo(criteria);

            // Assert
            targetTemplateService.Repository.Received().Update(Arg.Is<UpdateTargetTemplateBasicInfoCriteria>(x => x.IsValid()));
        }

        #endregion UpdateTargetTemplateBasicInfo

        #region DeleteTargetTemplate

        [Test]
        public void DeleteTargetTemplate_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeDeleteTargetTemplate();

            // Act
            var actual = targetTemplateService.DeleteTargetTemplate(new DeleteTargetTemplateCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void DeleteTargetTemplate_ValidArg_ExpectedToExecuteDelete()
        {
            // Arrange
            var criteria = new DeleteTargetTemplateCriteria
            {
                TargetTemplateId = 1,
                ModifiedBy = 1
            };
            ArrangeDeleteTargetTemplate();

            // Act
            targetTemplateService.DeleteTargetTemplate(criteria);

            // Assert
            targetTemplateService.Repository.Received().Delete(Arg.Is<DeleteTargetTemplateCriteria>(x => x.IsValid()));
        }

        #endregion DeleteTargetTemplate

        #region DeleteKPIGroup

        [Test]
        public void DeleteKPIGroup_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeDeleteKPIGroup();

            // Act
            var actual = targetTemplateService.DeleteKPIGroup(new DeleteKPIGroupOfTargetTemplateCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void DeleteKPIGroup_ValidArg_ExpectedToExecuteDelete()
        {
            // Arrange
            var criteria = new DeleteKPIGroupOfTargetTemplateCriteria
            {
                TargetTemplateId = 1,
                KPIGroupId = 1,
                ModifiedBy = 1
            };
            ArrangeDeleteKPIGroup();

            // Act
            targetTemplateService.DeleteKPIGroup(criteria);

            // Assert
            targetTemplateService.Repository.Received().Delete(Arg.Is<DeleteKPIGroupOfTargetTemplateCriteria>(x => x.IsValid()));
        }

        #endregion DeleteKPIGroup

        #region AddKPIGroupWeight

        [Test]
        public void AddKPIGroupWeight_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeAddKPIGroupWeight();

            // Act
            var actual = targetTemplateService.AddKPIGroupWeight(new SetKPIGroupOfTargetTemplateCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void AddKPIGroupWeight_ValidArg_ExpectedToExecuteInsert()
        {
            // Arrange
            var criteria = new SetKPIGroupOfTargetTemplateCriteria
            {
                TargetTemplateId = 1,
                ModifiedBy = 1
            };
            ArrangeAddKPIGroupWeight();

            // Act
            targetTemplateService.AddKPIGroupWeight(criteria);

            // Assert
            targetTemplateService.Repository.Received().Insert(Arg.Is<SetKPIGroupOfTargetTemplateCriteria>(x => x.IsValid()));
        }

        #endregion AddKPIGroupWeight

        #region GetKPIGroupOfTargetTemplate

        [Test]
        public void GetKPIGroupOfTargetTemplate_AnyArg_ReturnInstanceOfListOfKPIGroupModel()
        {
            // Arrange
            ArrangeGetKPIGroupOfTargetTemplate();

            // Act
            var actual = targetTemplateService.GetKPIGroupOfTargetTemplate(new GetKPIGroupOfTargetTemplateCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<KPIGroupModel>), actual);
        }

        [Test]
        public void GetKPIGroupOfTargetTemplate_ValidArg_ExpectedToExecuteGetData()
        {
            // Arrange
            var criteria = new GetKPIGroupOfTargetTemplateCriteria
            {
                TargetTemplateId = 1
            };
            ArrangeGetKPIGroupOfTargetTemplate();

            // Act
            targetTemplateService.GetKPIGroupOfTargetTemplate(criteria);

            // Assert
            targetTemplateService.Received().GetData<KPIGroupModel>(Arg.Is<GetKPIGroupOfTargetTemplateCriteria>(x => x.IsValid()));
        }

        [Test]
        public void GetKPIGroupOfTargetTemplate_CheckSelectedStatusKPIGroup_ReturnAsExpected()
        {
            // Arrange
            IEnumerable<KPIGroupModel> result = GetListOfKPIGroupModelForTest();
            ArrangeGetKPIGroupOfTargetTemplate(result);

            // Act
            var criteria = new GetKPIGroupOfTargetTemplateCriteria
            {
                TargetTemplateId = 1
            };
            var kpiGroups = targetTemplateService.GetKPIGroupOfTargetTemplate(criteria);

            // Assert
            Assert.AreEqual(1, kpiGroups.Count(x => x.Selected.HasValue && x.Selected.Value));
        }

        #endregion GetKPIGroupOfTargetTemplate

        #region Helpers

        private IBaseRepository FakeBaseRepository()
        {
            var repository = Substitute.For<IBaseRepository>();

            return repository;
        }

        private TargetTemplateService GetTargetTemplateService(IBaseRepository baseRepository = null)
        {
            var service = new TargetTemplateService();
            service.Repository = baseRepository;

            return service;
        }

        private void ArrangeGetTargetTemplateById(TargetTemplate result = null)
        {
            targetTemplateService = Substitute.For<TargetTemplateService>();
            targetTemplateService.When(x => x.GetDataEntity<TargetTemplate>(Arg.Any<GetTargetTemplateByIdCriteria>())).DoNotCallBase();
            targetTemplateService.GetDataEntity<TargetTemplate>(Arg.Any<GetTargetTemplateByIdCriteria>()).Returns(result ?? new TargetTemplate());
        }

        private void ArrangeGetAllTargetTemplate(IEnumerable<TargetTemplate> result = null)
        {
            targetTemplateService = Substitute.For<TargetTemplateService>();
            targetTemplateService.When(x => x.GetData<TargetTemplate>(Arg.Any<GetAllTargetTemplateCriteria>())).DoNotCallBase();
            targetTemplateService.GetData<TargetTemplate>(Arg.Any<GetAllTargetTemplateCriteria>()).Returns(result ?? new List<TargetTemplate>());
        }

        private void ArrangeGetRatingType(IEnumerable<RatingType> result = null)
        {
            targetTemplateService = Substitute.For<TargetTemplateService>();
            targetTemplateService.When(x => x.GetData<RatingType>(Arg.Any<GetRatingTypeCriteria>())).DoNotCallBase();
            targetTemplateService.GetData<RatingType>(Arg.Any<GetRatingTypeCriteria>()).Returns(result ?? new List<RatingType>());
        }

        private void ArrangeCreateTargetTemplate(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            targetTemplateService = GetTargetTemplateService(baseRepository);
            targetTemplateService.Repository.Insert(Arg.Any<CreateTargetTemplateCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeUpdateStatusTargetTemplate(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            targetTemplateService = GetTargetTemplateService(baseRepository);
            targetTemplateService.Repository.Update(Arg.Any<UpdateStatusTargetTemplateCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeUpdateTargetTemplate(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            targetTemplateService = GetTargetTemplateService(baseRepository);
            targetTemplateService.Repository.Update(Arg.Any<UpdateTargetTemplateCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeUpdateTargetTemplateBasicInfo(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            targetTemplateService = GetTargetTemplateService(baseRepository);
            targetTemplateService.Repository.Update(Arg.Any<UpdateTargetTemplateBasicInfoCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeDeleteTargetTemplate(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            targetTemplateService = GetTargetTemplateService(baseRepository);
            targetTemplateService.Repository.Delete(Arg.Any<DeleteTargetTemplateCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeDeleteKPIGroup(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            targetTemplateService = GetTargetTemplateService(baseRepository);
            targetTemplateService.Repository.Delete(Arg.Any<DeleteKPIGroupOfTargetTemplateCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeAddKPIGroupWeight(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            targetTemplateService = GetTargetTemplateService(baseRepository);
            targetTemplateService.Repository.Insert(Arg.Any<SetKPIGroupOfTargetTemplateCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeGetKPIGroupOfTargetTemplate(IEnumerable<KPIGroupModel> result = null)
        {
            targetTemplateService = Substitute.For<TargetTemplateService>();
            targetTemplateService.When(x => x.GetData<KPIGroupModel>(Arg.Any<GetKPIGroupOfTargetTemplateCriteria>())).DoNotCallBase();
            targetTemplateService.GetData<KPIGroupModel>(Arg.Any<GetKPIGroupOfTargetTemplateCriteria>()).Returns(result ?? new List<KPIGroupModel>());
        }

        private IEnumerable<KPIGroupModel> GetListOfKPIGroupModelForTest()
        {
            var groups = new List<KPIGroupModel>
            {
                new KPIGroupModel
                {
                    KPIGroupId = 1,
                    Name = "1"
                },
                new KPIGroupModel
                {
                    KPIGroupId = 2,
                    Name = "2",
                    Weight = 10
                }
            };

            return groups;
        }

        #endregion Helpers
    }
}