namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Repositories;
    using Criteria;
    using Implement;
    using Model;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class KPIGroupServiceTests
    {
        private KPIGroupService _kpiGroupService;

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

        #endregion Helpers
    }
}