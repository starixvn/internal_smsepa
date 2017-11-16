namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Services;
    using Criteria;
    using Implement;
    using Model;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class TargetTemplateServiceTests
    {
        private TargetTemplateService _targetTemplateService;
        private IBaseService baseService;

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
            baseService.Received().GetData<KPIGroupModel>(Arg.Is<GetKPIGroupOfTargetTemplateCriteria>(x => x.IsValid()));
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

        private IBaseService FakeBaseService()
        {
            var repository = Substitute.For<IBaseService>();

            return repository;
        }

        private TargetTemplateService GetTargetTemplateService(
            IBaseService baseService = null)
        {
            var service = new TargetTemplateService(null, baseService);

            return service;
        }

        private void ArrangeGetKPIGroupOfTargetTemplate(IEnumerable<KPIGroupModel> result = null)
        {
            baseService = FakeBaseService();
            baseService.GetData<KPIGroupModel>(Arg.Any<GetKPIGroupOfTargetTemplateCriteria>()).Returns(result ?? new List<KPIGroupModel>());
            _targetTemplateService = GetTargetTemplateService(baseService);
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