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
        private IBaseRepository baseRepository;

        #region GetTargetTemplateById

        [Test]
        public void GetTargetTemplateById_AnyArg_ReturnInstanceOfTargetTemplateModel()
        {
            // Arrange
            var service = Substitute.For<TargetTemplateService>();
            service.When(x => x.GetDataEntity<TargetTemplateModel>(Arg.Any<GetTargetTemplateByIdCriteria>())).DoNotCallBase();
            service.GetDataEntity<TargetTemplateModel>(Arg.Any<GetTargetTemplateByIdCriteria>()).Returns(new TargetTemplateModel());

            // Act
            var actual = service.GetTargetTemplateById(new GetTargetTemplateByIdCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(TargetTemplateModel), actual);
        }

        [Test]
        public void GetTargetTemplateById_ValidArg_ExpectedToExecuteGetDataEntity()
        {
            // Arrange
            var service = Substitute.For<TargetTemplateService>();
            var criteria = new GetTargetTemplateByIdCriteria
            {
                TargetTemplateId = 1
            };
            service.When(x => x.GetDataEntity<TargetTemplateModel>(Arg.Any<GetTargetTemplateByIdCriteria>())).DoNotCallBase();
            service.GetDataEntity<TargetTemplateModel>(Arg.Any<GetTargetTemplateByIdCriteria>()).Returns(new TargetTemplateModel());

            // Act
            service.GetTargetTemplateById(criteria);

            // Assert
            service.Received(1).GetDataEntity<TargetTemplateModel>(Arg.Is<GetTargetTemplateByIdCriteria>(x => x.IsValid()));
        }

        #endregion GetTargetTemplateById

        #region GetAllTargetTemplate

        [Test]
        public void GetAllTargetTemplate_AnyArg_ReturnInstanceOfListOfTargetTemplateModel()
        {
            // Arrange
            var service = Substitute.For<TargetTemplateService>();
            service.When(x => x.GetData<TargetTemplateModel>(Arg.Any<GetAllTargetTemplateCriteria>())).DoNotCallBase();
            service.GetData<TargetTemplateModel>(Arg.Any<GetAllTargetTemplateCriteria>()).Returns(new List<TargetTemplateModel>());

            // Act
            var actual = service.GetAllTargetTemplate(new GetAllTargetTemplateCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<TargetTemplateModel>), actual);
        }

        [Test]
        public void GetAllTargetTemplate_ValidArg_ExpectedToExecuteGetData()
        {
            // Arrange
            var service = Substitute.For<TargetTemplateService>();
            var criteria = new GetAllTargetTemplateCriteria();
            service.When(x => x.GetData<TargetTemplateModel>(Arg.Any<GetAllTargetTemplateCriteria>())).DoNotCallBase();
            service.GetData<TargetTemplateModel>(Arg.Any<GetAllTargetTemplateCriteria>()).Returns(new List<TargetTemplateModel>());

            // Act
            service.GetAllTargetTemplate(criteria);

            // Assert
            service.Received(1).GetData<TargetTemplateModel>(Arg.Is<GetAllTargetTemplateCriteria>(x => x.IsValid()));
        }

        #endregion GetAllTargetTemplate

        #region GetRatingType

        [Test]
        public void GetRatingType_AnyArg_ReturnInstanceOfListOfRatingType()
        {
            // Arrange
            var service = Substitute.For<TargetTemplateService>();
            service.When(x => x.GetData<RatingType>(Arg.Any<GetRatingTypeCriteria>())).DoNotCallBase();
            service.GetData<RatingType>(Arg.Any<GetRatingTypeCriteria>()).Returns(new List<RatingType>());

            // Act
            var actual = service.GetRatingType(new GetRatingTypeCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<RatingType>), actual);
        }

        [Test]
        public void GetRatingType_ValidArg_ExpectedToExecuteGetData()
        {
            // Arrange
            var service = Substitute.For<TargetTemplateService>();
            var criteria = new GetRatingTypeCriteria();
            service.When(x => x.GetData<RatingType>(Arg.Any<GetRatingTypeCriteria>())).DoNotCallBase();
            service.GetData<RatingType>(Arg.Any<GetRatingTypeCriteria>()).Returns(new List<RatingType>());

            // Act
            service.GetRatingType(criteria);

            // Assert
            service.Received(1).GetData<RatingType>(Arg.Is<GetRatingTypeCriteria>(x => x.IsValid()));
        }

        #endregion GetRatingType

        #region CreateTargetTemplate

        [Test]
        public void CreateTargetTemplate_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            baseRepository = FakeBaseRepository();
            var service = GetTargetTemplateService(baseRepository);
            service.Repository.Insert(Arg.Any<CreateTargetTemplateCriteria>()).Returns(new CommandResult());

            // Act
            var actual = service.CreateTargetTemplate(new CreateTargetTemplateCriteria());

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
            baseRepository = FakeBaseRepository();
            var service = GetTargetTemplateService(baseRepository);
            service.Repository.Insert(Arg.Any<CreateTargetTemplateCriteria>()).Returns(new CommandResult());

            // Act
            service.CreateTargetTemplate(criteria);

            // Assert
            service.Repository.Received(1).Insert(Arg.Is<CreateTargetTemplateCriteria>(x => x.IsValid()));
        }

        #endregion CreateTargetTemplate

        #region UpdateStatusTargetTemplate

        [Test]
        public void UpdateStatusTargetTemplate_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            baseRepository = FakeBaseRepository();
            var service = GetTargetTemplateService(baseRepository);
            service.Repository.Update(Arg.Any<UpdateStatusTargetTemplateCriteria>()).Returns(new CommandResult());

            // Act
            var actual = service.UpdateStatusTargetTemplate(new UpdateStatusTargetTemplateCriteria());

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
            baseRepository = FakeBaseRepository();
            var service = GetTargetTemplateService(baseRepository);
            service.Repository.Update(Arg.Any<UpdateStatusTargetTemplateCriteria>()).Returns(new CommandResult());

            // Act
            service.UpdateStatusTargetTemplate(criteria);

            // Assert
            service.Repository.Received(1).Update(Arg.Is<UpdateStatusTargetTemplateCriteria>(x => x.IsValid()));
        }

        #endregion UpdateStatusTargetTemplate

        #region UpdateTargetTemplate

        [Test]
        public void UpdateTargetTemplate_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            baseRepository = FakeBaseRepository();
            var service = GetTargetTemplateService(baseRepository);
            service.Repository.Update(Arg.Any<UpdateTargetTemplateCriteria>()).Returns(new CommandResult());

            // Act
            var actual = service.UpdateTargetTemplate(new UpdateTargetTemplateCriteria());

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
            baseRepository = FakeBaseRepository();
            var service = GetTargetTemplateService(baseRepository);
            service.Repository.Update(Arg.Any<UpdateTargetTemplateCriteria>()).Returns(new CommandResult());

            // Act
            service.UpdateTargetTemplate(criteria);

            // Assert
            service.Repository.Received(1).Update(Arg.Is<UpdateTargetTemplateCriteria>(x => x.IsValid()));
        }

        #endregion UpdateTargetTemplate

        #region UpdateTargetTemplateBasicInformation

        [Test]
        public void UpdateTargetTemplateBasicInformation_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            baseRepository = FakeBaseRepository();
            var service = GetTargetTemplateService(baseRepository);
            service.Repository.Update(Arg.Any<UpdateTargetTemplateBasicInformationCriteria>()).Returns(new CommandResult());

            // Act
            var actual = service.UpdateTargetTemplateBasicInformation(new UpdateTargetTemplateBasicInformationCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateTargetTemplateBasicInformation_ValidArg_ExpectedToExecuteUpdate()
        {
            // Arrange
            var criteria = new UpdateTargetTemplateBasicInformationCriteria
            {
                TargetTemplateId = 1,
                Name = "test",
                TargetTypeId = 1,
                ScoreRatingTypeId = 1,
                RatingTypeId = 1,
                ModifiedBy = 1
            };
            baseRepository = FakeBaseRepository();
            var service = GetTargetTemplateService(baseRepository);
            service.Repository.Update(Arg.Any<UpdateTargetTemplateBasicInformationCriteria>()).Returns(new CommandResult());

            // Act
            service.UpdateTargetTemplateBasicInformation(criteria);

            // Assert
            service.Repository.Received(1).Update(Arg.Is<UpdateTargetTemplateBasicInformationCriteria>(x => x.IsValid()));
        }

        #endregion UpdateTargetTemplateBasicInformation

        #region DeleteTargetTemplate

        [Test]
        public void DeleteTargetTemplate_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            baseRepository = FakeBaseRepository();
            var service = GetTargetTemplateService(baseRepository);
            service.Repository.Delete(Arg.Any<DeleteTargetTemplateCriteria>()).Returns(new CommandResult());

            // Act
            var actual = service.DeleteTargetTemplate(new DeleteTargetTemplateCriteria());

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
            baseRepository = FakeBaseRepository();
            var service = GetTargetTemplateService(baseRepository);
            service.Repository.Delete(Arg.Any<DeleteTargetTemplateCriteria>()).Returns(new CommandResult());

            // Act
            service.DeleteTargetTemplate(criteria);

            // Assert
            service.Repository.Received(1).Delete(Arg.Is<DeleteTargetTemplateCriteria>(x => x.IsValid()));
        }

        #endregion DeleteTargetTemplate

        #region DeleteKPIGroup

        [Test]
        public void DeleteKPIGroup_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            baseRepository = FakeBaseRepository();
            var service = GetTargetTemplateService(baseRepository);
            service.Repository.Delete(Arg.Any<DeleteKPIGroupOfTargetTemplateCriteria>()).Returns(new CommandResult());

            // Act
            var actual = service.DeleteKPIGroup(new DeleteKPIGroupOfTargetTemplateCriteria());

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
            baseRepository = FakeBaseRepository();
            var service = GetTargetTemplateService(baseRepository);
            service.Repository.Delete(Arg.Any<DeleteKPIGroupOfTargetTemplateCriteria>()).Returns(new CommandResult());

            // Act
            service.DeleteKPIGroup(criteria);

            // Assert
            service.Repository.Received(1).Delete(Arg.Is<DeleteKPIGroupOfTargetTemplateCriteria>(x => x.IsValid()));
        }

        #endregion DeleteKPIGroup

        #region AddKPIGroupWeight

        [Test]
        public void AddKPIGroupWeight_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            baseRepository = FakeBaseRepository();
            var service = GetTargetTemplateService(baseRepository);
            service.Repository.Insert(Arg.Any<SetKPIGroupOfTargetTemplateCriteria>()).Returns(new CommandResult());

            // Act
            var actual = service.AddKPIGroupWeight(new SetKPIGroupOfTargetTemplateCriteria());

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
            baseRepository = FakeBaseRepository();
            var service = GetTargetTemplateService(baseRepository);
            service.Repository.Insert(Arg.Any<SetKPIGroupOfTargetTemplateCriteria>()).Returns(new CommandResult());

            // Act
            service.AddKPIGroupWeight(criteria);

            // Assert
            service.Repository.Received(1).Insert(Arg.Is<SetKPIGroupOfTargetTemplateCriteria>(x => x.IsValid()));
        }

        #endregion AddKPIGroupWeight

        #region GetKPIGroupOfTargetTemplate

        [Test]
        public void GetKPIGroupOfTargetTemplate_AnyArg_ReturnInstanceOfListOfKPIGroupModel()
        {
            // Arrange
            var service = Substitute.For<TargetTemplateService>();
            service.When(x => x.GetData<KPIGroupModel>(Arg.Any<GetKPIGroupOfTargetTemplateCriteria>())).DoNotCallBase();
            service.GetData<KPIGroupModel>(Arg.Any<GetKPIGroupOfTargetTemplateCriteria>()).Returns(new List<KPIGroupModel>());

            // Act
            var actual = service.GetKPIGroupOfTargetTemplate(new GetKPIGroupOfTargetTemplateCriteria());

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
            var service = Substitute.For<TargetTemplateService>();
            service.When(x => x.GetData<KPIGroupModel>(Arg.Any<GetKPIGroupOfTargetTemplateCriteria>())).DoNotCallBase();
            service.GetData<KPIGroupModel>(Arg.Any<GetKPIGroupOfTargetTemplateCriteria>()).Returns(new List<KPIGroupModel>());

            // Act
            service.GetKPIGroupOfTargetTemplate(criteria);

            // Assert
            service.Received(1).GetData<KPIGroupModel>(Arg.Is<GetKPIGroupOfTargetTemplateCriteria>(x => x.IsValid()));
        }

        [Test]
        public void GetKPIGroupOfTargetTemplate_CheckSelectedStatusKPIGroup_ReturnAsExpected()
        {
            // Arrange
            var service = Substitute.For<TargetTemplateService>();
            IEnumerable<KPIGroupModel> result = GetListOfKPIGroupModelForTest();
            service.When(x => x.GetData<KPIGroupModel>(Arg.Any<GetKPIGroupOfTargetTemplateCriteria>())).DoNotCallBase();
            service.GetData<KPIGroupModel>(Arg.Any<GetKPIGroupOfTargetTemplateCriteria>()).Returns(result);

            // Act
            var criteria = new GetKPIGroupOfTargetTemplateCriteria
            {
                TargetTemplateId = 1
            };
            var kpiGroups = service.GetKPIGroupOfTargetTemplate(criteria);

            // Assert
            Assert.AreEqual(1, kpiGroups.Count(x => x.Selected.HasValue && x.Selected.Value));
        }

        #endregion GetKPIGroupOfTargetTemplate

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
    }
}