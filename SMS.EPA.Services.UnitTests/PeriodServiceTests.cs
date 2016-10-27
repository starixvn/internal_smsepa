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
        private IBaseRepository baseRepository;
        private IPeriodRepository periodRepository;
        private IManualMailService manualMailService;
        private IConfigurationManagerService configurationManager;

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

        [Test]
        public void GetPeriodEmployeeSettingById_ValidArg_ReturnExpectedSetting()
        {
            // Arrange
            var criteria = new GetPeriodEmployeeSettingCriteria
            {
                PeriodId = 1
            };
            IEnumerable<PeriodEmployeeSettingModel> result = GetListOfPeriodEmployeeSettingForTest();
            ArrangeGetPeriodEmployeeSettingById(result);

            // Act
            var actual = periodService.GetPeriodEmployeeSettingById(criteria);

            // Assert
            Assert.AreEqual(result, actual);
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

        #region UpdateEmployeeSettingOfPeriod

        [Test]
        public void UpdateEmployeeSettingOfPeriod_InvalidArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            var criteria = new UpdateEmployeeSettingOfPeriodCriteria
            {
                PeriodId = 0
            };
            ArrangeUpdateEmployeeSettingOfPeriod();

            // Act
            var actual = periodService.UpdateEmployeeSettingOfPeriod(criteria);

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateEmployeeSettingOfPeriod_UpdateSuccessfully_ExpectedNotSendMail()
        {
            // Arrange
            var criteria = new UpdateEmployeeSettingOfPeriodCriteria
            {
                PeriodId = 1,
                ModifiedBy = 1
            };
            var commandResult = new CommandResult
            {
                Code = 0
            };
            ArrangeUpdateEmployeeSettingOfPeriod(commandResult, "false");

            // Act
            var actual = periodService.UpdateEmployeeSettingOfPeriod(criteria);

            // Assert
            Assert.AreEqual(commandResult.Code, actual.Code);
            manualMailService.DidNotReceive().SendNotifyMail(Arg.Any<PeriodModel>(), Arg.Any<List<PeriodFlowModel>>());
        }

        [Test]
        public void UpdateEmployeeSettingOfPeriod_UpdateSuccessfully_ExpectedSendMail()
        {
            // Arrange
            var criteria = new UpdateEmployeeSettingOfPeriodCriteria
            {
                PeriodId = 1,
                ModifiedBy = 1
            };
            var commandResult = new CommandResult
            {
                Code = 0
            };
            ArrangeUpdateEmployeeSettingOfPeriod(commandResult, "true");

            // Act
            var actual = periodService.UpdateEmployeeSettingOfPeriod(criteria);

            // Assert
            Assert.AreEqual(commandResult.Code, actual.Code);
            manualMailService.Received().SendNotifyMail(Arg.Any<PeriodModel>(), Arg.Any<List<PeriodFlowModel>>());
        }

        [TestCase("true")]
        [TestCase("false")]
        [Test]
        public void UpdateEmployeeSettingOfPeriod_UpdateUnsuccessful_ExpectedNotSendMail(string appSetting)
        {
            // Arrange
            var criteria = new UpdateEmployeeSettingOfPeriodCriteria
            {
                PeriodId = 1,
                ModifiedBy = 1
            };
            var commandResult = new CommandResult
            {
                Code = 1
            };
            ArrangeUpdateEmployeeSettingOfPeriod(commandResult, appSetting);

            // Act
            var actual = periodService.UpdateEmployeeSettingOfPeriod(criteria);

            // Assert
            Assert.AreEqual(commandResult.Code, actual.Code);
            manualMailService.DidNotReceive().SendNotifyMail(Arg.Any<PeriodModel>(), Arg.Any<List<PeriodFlowModel>>());
        }

        #endregion UpdateEmployeeSettingOfPeriod

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

        #region GetEmployeeSettingByPeriodId

        [Test]
        public void GetEmployeeSettingByPeriodId_AnyArg_ReturnInstanceOfListOfPeriodFlowModel()
        {
            // Arrange
            ArrangeGetEmployeeSettingByPeriodId();

            // Act
            var actual = periodService.GetEmployeeSettingByPeriodId(Arg.Any<GetEmployeeSettingByPeriodIdCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<PeriodFlowModel>), actual);
        }

        #endregion GetEmployeeSettingByPeriodId

        #region GetProcessSettingByPeriodId

        [Test]
        public void GetProcessSettingByPeriodId_AnyArg_ReturnInstanceOfListOfFlowModel()
        {
            // Arrange
            int periodId = 1;
            ArrangeGetProcessSettingByPeriodId();

            // Act
            var actual = periodService.GetProcessSettingByPeriodId(periodId);

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<FlowModel>), actual);
        }

        [Test]
        public void GetProcessSettingByPeriodId_ValidArg_ReturnExpectedSettings()
        {
            // Arrange
            int periodId = 1;
            IEnumerable<FlowModel> result = GetProcessSettingForTest();
            ArrangeGetProcessSettingByPeriodId(result);

            // Act
            var actual = periodService.GetProcessSettingByPeriodId(periodId);

            // Assert
            Assert.AreEqual(result, actual);
        }

        #endregion GetProcessSettingByPeriodId

        #region GetEvaluationGlobalSettingByTargetTypeId

        [Test]
        public void GetEvaluationGlobalSettingByTargetTypeId_AnyArg_ReturnInstanceOfListOfFlowModel()
        {
            // Arrange
            int targetTypeId = 1;
            ArrangeGetEvaluationGlobalSettingByTargetTypeId();

            // Act
            var actual = periodService.GetEvaluationGlobalSettingByTargetTypeId(targetTypeId);

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<FlowModel>), actual);
        }

        [Test]
        public void GetEvaluationGlobalSettingByTargetTypeId_ValidArg_ReturnExpectedFlows()
        {
            // Arrange
            int targetTypeId = 1;
            IEnumerable<FlowModel> result = GetEvaluationGlobalSettingForTest();
            ArrangeGetEvaluationGlobalSettingByTargetTypeId(result);

            // Act
            var actual = periodService.GetEvaluationGlobalSettingByTargetTypeId(targetTypeId);

            // Assert
            Assert.AreEqual(result, actual);
        }

        #endregion GetEvaluationGlobalSettingByTargetTypeId

        #region UpdateProcessSettingPeriod

        [Test]
        public void UpdateProcessSettingPeriod_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeUpdateProcessSettingPeriod();

            // Act
            var actual = periodService.UpdateProcessSettingPeriod(Arg.Any<UpdateProcessSettingPeriodCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void UpdateProcessSettingPeriod_ValidArg_ExpectedToExecuteUpdate()
        {
            // Arrange
            var criteria = new UpdateProcessSettingPeriodCriteria
            {
                PeriodId = 1,
                ModifiedBy = 1,
                Processes = new List<FlowModel>()
            };
            ArrangeUpdateProcessSettingPeriod();

            // Act
            periodService.UpdateProcessSettingPeriod(criteria);

            // Assert
            periodService.Repository.Received().Update(Arg.Is<UpdateProcessSettingPeriodCriteria>(x => x.IsValid()));
        }

        #endregion UpdateProcessSettingPeriod

        #region CheckSelfProcessStarted

        [Test]
        public void CheckSelfProcessStarted_AnyArg_ReturnIntanceOfBoolean()
        {
            // Arrange
            ArrangeCheckSelfProcessStarted();

            // Act
            var actual = periodService.CheckSelfProcessStarted(Arg.Any<CheckSelfProcessStartedCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(bool), actual);
        }

        [Test]
        public void CheckSelfProcessStarted_ValidArg_ReturnAsExpected()
        {
            // Arrange
            var criteria = new CheckSelfProcessStartedCriteria
            {
                PeriodId = 1,
                EmployeeIds = "1,2"
            };
            ArrangeCheckSelfProcessStarted(true);

            // Act
            var actual = periodService.CheckSelfProcessStarted(criteria);

            // Assert
            Assert.IsTrue(actual);
        }

        #endregion CheckSelfProcessStarted

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

        private IManualMailService FakeManualMailService()
        {
            IManualMailService service = Substitute.For<IManualMailService>();

            return service;
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

        private IEnumerable<PeriodEmployeeSettingModel> GetListOfPeriodEmployeeSettingForTest()
        {
            var settings = new List<PeriodEmployeeSettingModel>
            {
                new PeriodEmployeeSettingModel
                {
                    PeriodId = 1,
                    Name = "test"
                }
            };

            return settings;
        }

        private IEnumerable<FlowModel> GetProcessSettingForTest()
        {
            var settings = new List<FlowModel>
            {
                new FlowModel
                {
                    ProcessId = 1,
                    Duration = 1,
                    RemindDay = 1
                },
                new FlowModel
                {
                    ProcessId = 2,
                    Duration = 1,
                    RemindDay = 1
                }
            };

            return settings;
        }

        private IEnumerable<FlowModel> GetEvaluationGlobalSettingForTest()
        {
            var settings = new List<FlowModel>
            {
                new FlowModel
                {
                    ProcessId = 1,
                    Duration = 1,
                    RemindDay = 1
                },
                new FlowModel
                {
                    ProcessId = 2,
                    Duration = 1,
                    RemindDay = 1
                }
            };

            return settings;
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

        private void ArrangeGetEmployeeSettingByPeriodId(IEnumerable<PeriodFlowModel> result = null)
        {
            periodRepository = FakePeriodRepository();
            periodRepository.GetEmployeeSettingByPeriodId(Arg.Any<GetEmployeeSettingByPeriodIdCriteria>()).Returns(result ?? new List<PeriodFlowModel>());
            periodService = GetPeriodService(null, periodRepository);
        }

        private void ArrangeGetProcessSettingByPeriodId(IEnumerable<FlowModel> result = null)
        {
            periodService = Substitute.For<PeriodService>();
            periodService.GetData<FlowModel>(Arg.Any<GetProcessSettingPeriodCriteria>()).Returns(result ?? new List<FlowModel>());
        }

        private void ArrangeGetEvaluationGlobalSettingByTargetTypeId(IEnumerable<FlowModel> result = null)
        {
            periodService = Substitute.For<PeriodService>();
            periodService.GetData<FlowModel>(Arg.Any<GetEvaluationGlobalSettingByTargetTypeIdCriteria>()).Returns(result ?? new List<FlowModel>());
        }

        private void ArrangeUpdateProcessSettingPeriod(CommandResult result = null)
        {
            baseRepository = FakeBaseRepository();
            periodService = GetPeriodService(baseRepository);
            periodService.Repository.Update(Arg.Any<UpdateProcessSettingPeriodCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeCheckSelfProcessStarted(bool result = false)
        {
            periodService = Substitute.For<PeriodService>();
            periodService.GetDataEntity<bool>(Arg.Any<CheckSelfProcessStartedCriteria>()).Returns(result);
        }

        private void ArrangeUpdateEmployeeSettingOfPeriod(CommandResult result = null, string appSetting = null)
        {
            var period = new PeriodModel();
            var employeeSettings = new List<PeriodFlowModel>();

            baseRepository = FakeBaseRepository();
            configurationManager = FakeConfigurationManagerService();
            manualMailService = FakeManualMailService();
            periodRepository = FakePeriodRepository();

            periodService = GetPeriodService(baseRepository, periodRepository, manualMailService, configurationManager);
            periodService.Repository.Update(Arg.Any<UpdateEmployeeSettingOfPeriodCriteria>()).Returns(result ?? new CommandResult());
            configurationManager.GetAppSetting(Arg.Any<string>()).Returns(appSetting ?? string.Empty);
            periodRepository.GetPeriodById(Arg.Any<GetPeriodByIdCriteria>()).Returns(period);
            periodRepository.GetEmployeeSettingByPeriodId(Arg.Any<GetEmployeeSettingByPeriodIdCriteria>()).Returns(employeeSettings);
        }

        #endregion Helpers
    }
}