namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Repositories;
    using Criteria;
    using Implement;
    using Model;
    using Model.Enitites;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class TargetTemplateServiceTests
    {
        private TargetTemplateService _targetTemplateService;

        #region GetTargetTemplateById

        [Test]
        public void GetTargetTemplateById_AnyArg_ReturnInstanceOfTargetTemplateModel()
        {
            // Arrange
            ArrangeGetTargetTemplateById();

            // Act
            var actual = _targetTemplateService.GetTargetTemplateById(new GetTargetTemplateByIdCriteria());

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
            _targetTemplateService.GetTargetTemplateById(criteria);

            // Assert
            _targetTemplateService.Received().GetDataEntity<TargetTemplate>(Arg.Is<GetTargetTemplateByIdCriteria>(x => x.IsValid()));
        }

        #endregion GetTargetTemplateById

        #region GetAllTargetTemplate

        [Test]
        public void GetAllTargetTemplate_AnyArg_ReturnInstanceOfListOfTargetTemplateModel()
        {
            // Arrange
            ArrangeGetAllTargetTemplate();

            // Act
            var actual = _targetTemplateService.GetAllTargetTemplate(new GetAllTargetTemplateCriteria());

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
            _targetTemplateService.GetAllTargetTemplate(criteria);

            // Assert
            _targetTemplateService.Received().GetData<TargetTemplate>(Arg.Is<GetAllTargetTemplateCriteria>(x => x.IsValid()));
        }

        #endregion GetAllTargetTemplate

        #region GetRatingType

        [Test]
        public void GetRatingType_AnyArg_ReturnInstanceOfListOfRatingType()
        {
            // Arrange
            ArrangeGetRatingType();

            // Act
            var actual = _targetTemplateService.GetRatingType(new GetRatingTypeCriteria());

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
            _targetTemplateService.GetRatingType(criteria);

            // Assert
            _targetTemplateService.Received().GetData<RatingType>(Arg.Is<GetRatingTypeCriteria>(x => x.IsValid()));
        }

        #endregion GetRatingType

        #region GetKPIGroupOfTargetTemplate

        [Test]
        public void GetKPIGroupOfTargetTemplate_AnyArg_ReturnInstanceOfListOfKPIGroupModel()
        {
            // Arrange
            ArrangeGetKPIGroupOfTargetTemplate();

            // Act
            var actual = _targetTemplateService.GetKPIGroupOfTargetTemplate(new GetKPIGroupOfTargetTemplateCriteria());

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
            _targetTemplateService.GetKPIGroupOfTargetTemplate(criteria);

            // Assert
            _targetTemplateService.Received().GetData<KPIGroupModel>(Arg.Is<GetKPIGroupOfTargetTemplateCriteria>(x => x.IsValid()));
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
            var kpiGroups = _targetTemplateService.GetKPIGroupOfTargetTemplate(criteria);

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
            _targetTemplateService = Substitute.For<TargetTemplateService>();
            _targetTemplateService.When(x => x.GetDataEntity<TargetTemplate>(Arg.Any<GetTargetTemplateByIdCriteria>())).DoNotCallBase();
            _targetTemplateService.GetDataEntity<TargetTemplate>(Arg.Any<GetTargetTemplateByIdCriteria>()).Returns(result ?? new TargetTemplate());
        }

        private void ArrangeGetAllTargetTemplate(IEnumerable<TargetTemplate> result = null)
        {
            _targetTemplateService = Substitute.For<TargetTemplateService>();
            _targetTemplateService.When(x => x.GetData<TargetTemplate>(Arg.Any<GetAllTargetTemplateCriteria>())).DoNotCallBase();
            _targetTemplateService.GetData<TargetTemplate>(Arg.Any<GetAllTargetTemplateCriteria>()).Returns(result ?? new List<TargetTemplate>());
        }

        private void ArrangeGetRatingType(IEnumerable<RatingType> result = null)
        {
            _targetTemplateService = Substitute.For<TargetTemplateService>();
            _targetTemplateService.When(x => x.GetData<RatingType>(Arg.Any<GetRatingTypeCriteria>())).DoNotCallBase();
            _targetTemplateService.GetData<RatingType>(Arg.Any<GetRatingTypeCriteria>()).Returns(result ?? new List<RatingType>());
        }

        private void ArrangeGetKPIGroupOfTargetTemplate(IEnumerable<KPIGroupModel> result = null)
        {
            _targetTemplateService = Substitute.For<TargetTemplateService>();
            _targetTemplateService.When(x => x.GetData<KPIGroupModel>(Arg.Any<GetKPIGroupOfTargetTemplateCriteria>())).DoNotCallBase();
            _targetTemplateService.GetData<KPIGroupModel>(Arg.Any<GetKPIGroupOfTargetTemplateCriteria>()).Returns(result ?? new List<KPIGroupModel>());
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