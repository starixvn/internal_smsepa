namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Repositories;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Repositories;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    [TestFixture]
    public class PeriodServiceTests
    {
        private PeriodService periodService;

        ////private IManualMailService manualMailService;
        ////private IConfigurationManagerService configurationManager;
        private IBaseRepository baseRepository;

        private IPeriodRepository periodRepository;

        #region GetPeriod

        [Test]
        public void GetPeriod_AnyArg_ReturnInstanceOfListOfPeriodModel()
        {
            // Arrange
            ArrangeGetPeriod();

            // Act
            var actual = periodService.GetPeriod(Arg.Any<GetPeriodCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<PeriodModel>), actual);
        }

        [Test]
        public void GetPeriod_ValidArg_ReturnExpectedPeriods()
        {
            // Arrange
            var criteria = new GetPeriodCriteria();
            IEnumerable<PeriodModel> result = GetListOfPeriodForTest();
            ArrangeGetPeriod(result);

            // Act
            var actual = periodService.GetPeriod(criteria);

            // Assert
            Assert.AreEqual(result, actual);
        }

        [Test]
        public void GetPeriod_ValidArg_PeriodIncludeTargetType()
        {
            // Arrange
            var criteria = new GetPeriodCriteria();
            IEnumerable<PeriodModel> result = GetListOfPeriodForTest();
            ArrangeGetPeriod(result);

            // Act
            var actual = periodService.GetPeriod(criteria);

            // Assert
            Assert.AreEqual(0, actual.Where(x => x.TargetType == null || x.TargetType.TargetTypeId <= 0).Count());
        }

        [Test]
        public void GetPeriod_ValidArg_ResultListingNotContainMonthlyTargetType()
        {
            // Arrange
            var criteria = new GetPeriodCriteria();
            IEnumerable<PeriodModel> result = GetListOfPeriodForTest();
            ArrangeGetPeriod(result);

            // Act
            var actual = periodService.GetPeriod(criteria);

            // Assert
            Assert.IsFalse(actual.Any(x => x.TargetType.TargetTypeId == (int)TargetTypes.Monthly));
        }

        #endregion GetPeriod

        #region GetPeriodById

        [Test]
        public void GetPeriodById_AnyArg_ReturnInstanceOfPeriodModel()
        {
            // Arrange
            int periodId = 1;
            ArrangeGetPeriodById();

            // Act
            var actual = periodService.GetPeriodById(periodId);

            // Assert
            Assert.IsInstanceOf(typeof(PeriodModel), actual);
        }

        [Test]
        public void GetPeriodById_ValidArg_ReturnExpectedPeriod()
        {
            // Arrange
            int periodId = 1;
            PeriodModel result = GetListOfPeriodForTest().First();
            ArrangeGetPeriodById(result);

            // Act
            var actual = periodService.GetPeriodById(periodId);

            // Assert
            Assert.AreEqual(result, actual);
            Assert.AreEqual(periodId, actual.PeriodId);
        }

        [Test]
        public void GetPeriodById_ValidArg_PeriodIncludeTargetType()
        {
            // Arrange
            int periodId = 1;
            PeriodModel result = GetListOfPeriodForTest().First();
            ArrangeGetPeriodById(result);

            // Act
            var actual = periodService.GetPeriodById(periodId);

            // Assert
            Assert.AreEqual(false, actual.TargetType == null || actual.TargetType.TargetTypeId <= 0);
        }

        [Test]
        public void GetPeriodById_ValidArg_PeriodNotContainMonthlyTargetType()
        {
            // Arrange
            int periodId = 1;
            PeriodModel result = GetListOfPeriodForTest().First();
            ArrangeGetPeriodById(result);

            // Act
            var actual = periodService.GetPeriodById(periodId);

            // Assert
            Assert.IsTrue(actual.TargetType.TargetTypeId != (int)TargetTypes.Monthly);
        }

        #endregion GetPeriodById

        #region CreatePeriod

        [Test]
        public void CreatePeriod_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeCreatePeriod();

            // Act
            var actual = periodService.CreatePeriod(new CreatePeriodCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void CreatePeriod_ValidArg_ExpectedToExecuteInsert()
        {
            // Arrange
            var criteria = new CreatePeriodCriteria
            {
                Name = "test",
                TargetTypeId = 1,
                CreatedBy = 1
            };
            ArrangeCreatePeriod();

            // Act
            periodService.CreatePeriod(criteria);

            // Assert
            periodService.Repository.Received().Insert(Arg.Is<CreatePeriodCriteria>(x => x.IsValid()));
        }

        #endregion CreatePeriod

        #region AutoCreateMonthlyPeriod

        [Test]
        public void AutoCreateMonthlyPeriod_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeAutoCreateMonthlyPeriod();

            // Act
            var actual = periodService.AutoCreateMonthlyPeriod(new AutoCreateMonthlyPeriodCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void AutoCreateMonthlyPeriod_ValidArg_ExpectedToExecuteInsert()
        {
            // Arrange
            var criteria = new AutoCreateMonthlyPeriodCriteria
            {
                Name = "test"
            };
            ArrangeAutoCreateMonthlyPeriod();

            // Act
            periodService.AutoCreateMonthlyPeriod(criteria);

            // Assert
            periodService.Repository.Received().Insert(Arg.Is<AutoCreateMonthlyPeriodCriteria>(x => x.IsValid()));
        }

        #endregion AutoCreateMonthlyPeriod

        #region UpdateBasicInformationPeriod

        [Test]
        public void UpdateBasicInformationPeriod_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeUpdateBasicInformationPeriod();

            // Act
            var actual = periodService.UpdateBasicInformationPeriod(new UpdateBasicInformationPeriodCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateBasicInformationPeriod_ValidArg_ExpectedToExecuteUpdate()
        {
            // Arrange
            var criteria = new UpdateBasicInformationPeriodCriteria
            {
                PeriodId = 1,
                Name = "test",
                TargetTypeId = 1,
                ModifiedBy = 1
            };
            ArrangeAutoCreateMonthlyPeriod();

            // Act
            periodService.UpdateBasicInformationPeriod(criteria);

            // Assert
            periodService.Repository.Received().Update(Arg.Is<UpdateBasicInformationPeriodCriteria>(x => x.IsValid()));
        }

        #endregion UpdateBasicInformationPeriod

        #region UpdateStatusPeriod

        [Test]
        public void UpdateStatusPeriod_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeUpdateStatusPeriod();

            // Act
            var actual = periodService.UpdateStatusPeriod(new UpdateStatusPeriodCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateStatusPeriod_ValidArg_ExpectedToExecuteUpdate()
        {
            // Arrange
            var criteria = new UpdateStatusPeriodCriteria
            {
                PeriodId = 1,
                ModifiedBy = 1
            };
            ArrangeUpdateStatusPeriod();

            // Act
            periodService.UpdateStatusPeriod(criteria);

            // Assert
            periodService.Repository.Received().Update(Arg.Is<UpdateStatusPeriodCriteria>(x => x.IsValid()));
        }

        #endregion UpdateStatusPeriod

        #region GetPeriodEmployeeSettingById

        [Test]
        public void GetPeriodEmployeeSettingById_AnyArg_ReturnInstanceOfListOfPeriodEmployeeSettingModel()
        {
            // Arrange
            ArrangeGetPeriodEmployeeSettingById();

            // Act
            var actual = periodService.GetPeriodEmployeeSettingById(Arg.Any<GetPeriodEmployeeSettingCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<PeriodEmployeeSettingModel>), actual);
        }

        #endregion GetPeriodEmployeeSettingById

        #region DeletePeriod

        [Test]
        public void DeletePeriod_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeDeletePeriod();

            // Act
            var actual = periodService.DeletePeriod(new DeletePeriodCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void DeletePeriod_ValidArg_ExpectedToExecuteDelete()
        {
            // Arrange
            var criteria = new DeletePeriodCriteria
            {
                PeriodId = 1,
                ModifiedBy = 1
            };
            ArrangeDeletePeriod();

            // Act
            periodService.DeletePeriod(criteria);

            // Assert
            periodService.Repository.Received().Delete(Arg.Is<DeletePeriodCriteria>(x => x.IsValid()));
        }

        #endregion DeletePeriod

        #region AddEmployeeToPeriod

        [Test]
        public void AddEmployeeToPeriod_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeAddEmployeeToPeriod();

            // Act
            var actual = periodService.AddEmployeeToPeriod(new AddEmployeeToPeriodCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void AddEmployeeToPeriod_ValidArg_ExpectedToExecuteUpdate()
        {
            // Arrange
            var criteria = new AddEmployeeToPeriodCriteria
            {
                PeriodId = 1,
                ModifiedBy = 1
            };
            ArrangeAddEmployeeToPeriod();

            // Act
            periodService.AddEmployeeToPeriod(criteria);

            // Assert
            periodService.Repository.Received().Update(Arg.Is<AddEmployeeToPeriodCriteria>(x => x.IsValid()));
        }

        #endregion AddEmployeeToPeriod

        #region UpdateIncludeStatusEmployeePeriod

        [Test]
        public void UpdateIncludeStatusEmployeePeriod_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeUpdateIncludeStatusEmployeePeriod();

            // Act
            var actual = periodService.UpdateIncludeStatusEmployeePeriod(new UpdateIncludeStatusEmployeePeriodCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateIncludeStatusEmployeePeriod_ValidArg_ExpectedToExecuteUpdate()
        {
            // Arrange
            var criteria = new UpdateIncludeStatusEmployeePeriodCriteria
            {
                PeriodId = 1,
                EmployeeId = 1,
                ModifiedBy = 1
            };
            ArrangeUpdateIncludeStatusEmployeePeriod();

            // Act
            periodService.UpdateIncludeStatusEmployeePeriod(criteria);

            // Assert
            periodService.Repository.Received().Update(Arg.Is<UpdateIncludeStatusEmployeePeriodCriteria>(x => x.IsValid()));
        }

        #endregion UpdateIncludeStatusEmployeePeriod

        #region GenerateEvaluation

        [Test]
        public void GenerateEvaluation_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeGenerateEvaluation();

            // Act
            var actual = periodService.GenerateEvaluation(new AutoGenerateEvaluationCriteria());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void GenerateEvaluation_ValidArg_ExpectedToExecuteInsert()
        {
            // Arrange
            var criteria = new AutoGenerateEvaluationCriteria
            {
                PeriodId = 1
            };
            ArrangeGenerateEvaluation();

            // Act
            periodService.GenerateEvaluation(criteria);

            // Assert
            periodService.Repository.Received().Insert(Arg.Is<AutoGenerateEvaluationCriteria>(x => x.IsValid()));
        }

        #endregion GenerateEvaluation

        #region Helpers

        private IBaseRepository FakeBaseRepository()
        {
            IBaseRepository repository = Substitute.For<IBaseRepository>();

            return repository;
        }

        private IPeriodRepository FakePeriodRepository()
        {
            IPeriodRepository repository = Substitute.For<IPeriodRepository>();

            return repository;
        }

        private IConfigurationManagerService FakeConfigurationManagerService()
        {
            IConfigurationManagerService service = Substitute.For<IConfigurationManagerService>();

            return service;
        }

        private PeriodService GetPeriodService(
            IBaseRepository baseRepository = null,
            IPeriodRepository periodRepository = null,
            IManualMailService manualMailService = null,
            IConfigurationManagerService configurationManager = null)
        {
            var service = new PeriodService(periodRepository, manualMailService, configurationManager);
            service.Repository = baseRepository;

            return service;
        }

        private IEnumerable<PeriodModel> GetListOfPeriodForTest()
        {
            var periods = new List<PeriodModel>
            {
                new PeriodModel
                {
                    PeriodId = 1,
                    Name = "test 1",
                    TargetType = new TargetType
                    {
                        TargetTypeId = 1,
                        Name = "Probation"
                    }
                },
                new PeriodModel
                {
                    PeriodId = 2,
                    Name = "test 2",
                    TargetType = new TargetType
                    {
                        TargetTypeId = 3,
                        Name = "Semi-annual"
                    }
                }
            };

            return periods;
        }

        private void ArrangeGetPeriod(IEnumerable<PeriodModel> result = null)
        {
            periodRepository = FakePeriodRepository();
            periodRepository.GetPeriod(Arg.Any<GetPeriodCriteria>()).Returns(result ?? new List<PeriodModel>());
            periodService = GetPeriodService(null, periodRepository);
        }

        public void ArrangeGetPeriodById(PeriodModel result = null)
        {
            periodRepository = FakePeriodRepository();
            periodRepository.GetPeriodById(Arg.Any<GetPeriodByIdCriteria>()).Returns(result ?? new PeriodModel());
            periodService = GetPeriodService(null, periodRepository);
        }

        private void ArrangeCreatePeriod(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            periodService = GetPeriodService(baseRepository);
            periodService.Repository.Insert(Arg.Any<CreatePeriodCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeAutoCreateMonthlyPeriod(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            periodService = GetPeriodService(baseRepository);
            periodService.Repository.Insert(Arg.Any<AutoCreateMonthlyPeriodCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeUpdateBasicInformationPeriod(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            periodService = GetPeriodService(baseRepository);
            periodService.Repository.Update(Arg.Any<UpdateBasicInformationPeriodCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeUpdateStatusPeriod(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            periodService = GetPeriodService(baseRepository);
            periodService.Repository.Update(Arg.Any<UpdateStatusPeriodCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeDeletePeriod(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            periodService = GetPeriodService(baseRepository);
            periodService.Repository.Delete(Arg.Any<DeletePeriodCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeGetPeriodEmployeeSettingById(IEnumerable<PeriodEmployeeSettingModel> result = null)
        {
            periodRepository = FakePeriodRepository();
            periodRepository.GetPeriodEmployeeSettingById(Arg.Any<GetPeriodEmployeeSettingCriteria>()).Returns(result ?? new List<PeriodEmployeeSettingModel>());
            periodService = GetPeriodService(null, periodRepository);
        }

        private void ArrangeAddEmployeeToPeriod(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            periodService = GetPeriodService(baseRepository);
            periodService.Repository.Update(Arg.Any<AddEmployeeToPeriodCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeUpdateIncludeStatusEmployeePeriod(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            periodService = GetPeriodService(baseRepository);
            periodService.Repository.Update(Arg.Any<UpdateIncludeStatusEmployeePeriodCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeGenerateEvaluation(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            periodService = GetPeriodService(baseRepository);
            periodService.Repository.Insert(Arg.Any<AutoGenerateEvaluationCriteria>()).Returns(result ?? new CommandResult());
        }

        #endregion Helpers
    }
}