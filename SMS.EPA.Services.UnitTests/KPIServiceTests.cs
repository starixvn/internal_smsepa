namespace SMS.EPA.Services.UnitTests
{
    using Core.Model;
    using Core.Repositories;
    using Criteria;
    using Implement;
    using Model;
    using NSubstitute;
    using NUnit.Framework;
    using Repositories;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class KPIServiceTests
    {
        private KPIService _kpiService;
        private IKPIRepository _kpiRepository;
        private IBaseRepository _baseRepository;

        #region GetKPI

        [Test]
        public void GetKPI_AnyArg_ReturnInstanceOfListOfKPIModel()
        {
            // Arrange
            ArrangeGetKPI();

            // Act
            var actual = _kpiService.GetKPI(new GetKPICriteria());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<KPIModel>), actual);
        }

        [Test]
        public void GetKPI_ValidArg_ReturnExpectedListOfKPIModel()
        {
            // Arrange
            var criteria = new GetKPICriteria();
            IEnumerable<KPIModel> result = GetListOfKPIForTest();
            ArrangeGetKPI(result);

            // Act
            IEnumerable<KPIModel> actual = _kpiService.GetKPI(criteria);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(result, actual);
        }

        [Test]
        public void GetKPI_ValidArg_KPIIncludeKPIGroup()
        {
            // Arrange
            var criteria = new GetKPICriteria();
            IEnumerable<KPIModel> result = GetListOfKPIForTest();
            ArrangeGetKPI(result);

            // Act
            IEnumerable<KPIModel> actual = _kpiService.GetKPI(criteria);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Where(x => !x.KPIGroups.Any()).Count());
        }

        #endregion GetKPI

        #region CreateKPI

        [Test]
        public void CreateKPI_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeCreateKPI();

            // Act
            var actual = _kpiService.CreateKPI(new CreateKPICriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void CreateKPI_ValidArg_ExpectedToExecuteInsert()
        {
            // Arrange
            var criteria = new CreateKPICriteria
            {
                Name = "test",
                KPIGroupIds = "1,2",
                CreatedBy = 1
            };
            ArrangeCreateKPI();

            // Act
            _kpiService.CreateKPI(criteria);

            // Assert
            _kpiService.Repository.Received().Insert(Arg.Is<CreateKPICriteria>(x => x.IsValid()));
        }

        #endregion CreateKPI

        #region UpdateKPI

        [Test]
        public void UpdateKPI_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeUpdateKPI();

            // Act
            var actual = _kpiService.UpdateKPI(new UpdateKPICriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateKPI_ValidArg_ExpectedToExecuteUpdate()
        {
            // Arrange
            var criteria = new UpdateKPICriteria
            {
                KPIId = 1,
                Name = "test",
                KPIGroupIds = "1,2",
                ModifiedBy = 1
            };
            ArrangeUpdateKPI();

            // Act
            _kpiService.UpdateKPI(criteria);

            // Assert
            _kpiService.Repository.Received().Update(Arg.Is<UpdateKPICriteria>(x => x.IsValid()));
        }

        #endregion UpdateKPI

        #region UpdateStatusKPI

        [Test]
        public void UpdateStatusKPI_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeUpdateStatusKPI();

            // Act
            var actual = _kpiService.UpdateStatusKPI(new UpdateStatusKPICriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateStatusKPI_ValidArg_ExpectedToExecuteUpdate()
        {
            // Arrange
            var criteria = new UpdateStatusKPICriteria
            {
                KPIId = 1,
                ModifiedBy = 1
            };
            ArrangeUpdateStatusKPI();

            // Act
            _kpiService.UpdateStatusKPI(criteria);

            // Assert
            _kpiService.Repository.Received().Update(Arg.Is<UpdateStatusKPICriteria>(x => x.IsValid()));
        }

        #endregion UpdateStatusKPI

        #region UpdateRequiredEvaluateKPI

        [Test]
        public void UpdateRequiredEvaluateKPI_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeUpdateRequiredEvaluateKPI();

            // Act
            var actual = _kpiService.UpdateRequiredEvaluateKPI(new UpdateRequiredEvaluateKPICriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateRequiredEvaluateKPI_ValidArg_ExpectedToExecuteUpdate()
        {
            // Arrange
            var criteria = new UpdateRequiredEvaluateKPICriteria
            {
                KPIId = 1,
                ModifiedBy = 1
            };
            ArrangeUpdateRequiredEvaluateKPI();

            // Act
            _kpiService.UpdateRequiredEvaluateKPI(criteria);

            // Assert
            _kpiService.Repository.Received().Update(Arg.Is<UpdateRequiredEvaluateKPICriteria>(x => x.IsValid()));
        }

        #endregion UpdateRequiredEvaluateKPI

        #region DeleteKPI

        [Test]
        public void DeleteKPI_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeDeleteKPI();

            // Act
            var actual = _kpiService.DeleteKPI(new DeleteKPICriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void DeleteKPI_ValidArg_ExpectedToExecuteDelete()
        {
            // Arrange
            var criteria = new DeleteKPICriteria
            {
                KPIId = 1,
                ModifiedBy = 1
            };
            ArrangeDeleteKPI();

            // Act
            _kpiService.DeleteKPI(criteria);

            // Assert
            _kpiService.Repository.Received().Delete(Arg.Is<DeleteKPICriteria>(x => x.IsValid()));
        }

        #endregion DeleteKPI

        #region Helpers

        private IBaseRepository FakeBaseRepository()
        {
            var repository = Substitute.For<IBaseRepository>();

            return repository;
        }

        private IKPIRepository FakeKPIRepository()
        {
            var repository = Substitute.For<IKPIRepository>();

            return repository;
        }

        private KPIService GetKPIService(IKPIRepository kpiRepository = null, IBaseRepository baseRepository = null)
        {
            var service = new KPIService(kpiRepository)
            {
                Repository = baseRepository
            };

            return service;
        }

        private IEnumerable<KPIModel> GetListOfKPIForTest()
        {
            var kpis = new List<KPIModel>
            {
                new KPIModel
                {
                    KPIId = 1,
                    Name = "test 1",
                    KPIGroups = new List<KPIGroupModel>
                    {
                        new KPIGroupModel
                        {
                            KPIGroupId = 1,
                            Name = "group test 1"
                        }
                    }
                },
                new KPIModel
                {
                    KPIId = 2,
                    Name = "test 2",
                    KPIGroups = new List<KPIGroupModel>
                    {
                        new KPIGroupModel
                        {
                            KPIGroupId = 1,
                            Name = "group test 1"
                        }
                    }
                }
            };

            return kpis;
        }

        private void ArrangeGetKPI(IEnumerable<KPIModel> result = null)
        {
            _kpiRepository = FakeKPIRepository();
            _kpiRepository.GetKPI(Arg.Any<GetKPICriteria>()).Returns(result ?? new List<KPIModel>());
            _kpiService = GetKPIService(_kpiRepository);
        }

        private void ArrangeCreateKPI(CommandResult result = null)
        {
            _baseRepository = FakeBaseRepository();
            _kpiService = GetKPIService(null, _baseRepository);
            _kpiService.Repository.Insert(Arg.Any<CreateKPICriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeUpdateKPI(CommandResult result = null)
        {
            _baseRepository = FakeBaseRepository();
            _kpiService = GetKPIService(null, _baseRepository);
            _kpiService.Repository.Update(Arg.Any<UpdateKPICriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeUpdateStatusKPI(CommandResult result = null)
        {
            _baseRepository = FakeBaseRepository();
            _kpiService = GetKPIService(null, _baseRepository);
            _kpiService.Repository.Update(Arg.Any<UpdateStatusKPICriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeUpdateRequiredEvaluateKPI(CommandResult result = null)
        {
            _baseRepository = FakeBaseRepository();
            _kpiService = GetKPIService(null, _baseRepository);
            _kpiService.Repository.Update(Arg.Any<UpdateRequiredEvaluateKPICriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeDeleteKPI(CommandResult result = null)
        {
            _baseRepository = FakeBaseRepository();
            _kpiService = GetKPIService(null, _baseRepository);
            _kpiService.Repository.Delete(Arg.Any<DeleteKPICriteria>()).Returns(result ?? new CommandResult());
        }

        #endregion Helpers
    }
}