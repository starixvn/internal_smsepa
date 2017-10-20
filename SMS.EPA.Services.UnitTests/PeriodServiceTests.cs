namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Enumerations;
    using Core.Model;
    using Core.Repositories;
    using Criteria;
    using Implement;
    using Interfaces;
    using Model;
    using NSubstitute;
    using NUnit.Framework;
    using Repositories;

    [TestFixture]
    public class PeriodServiceTests
    {
        private PeriodService _periodService;
        private IBaseRepository _baseRepository;
        private IPeriodRepository _periodRepository;
        private IPeriodMailService _manualMailService;

        #region GetPeriod

        [Test]
        public void GetPeriod_AnyArg_ReturnInstanceOfListOfPeriodModel()
        {
            // Arrange
            ArrangeGetPeriod();

            // Act
            var actual = _periodService.GetPeriod(Arg.Any<GetPeriodCriteria>());

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
            var actual = _periodService.GetPeriod(criteria);

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
            var actual = _periodService.GetPeriod(criteria);

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
            var actual = _periodService.GetPeriod(criteria);

            // Assert
            Assert.IsFalse(actual.Any(x => x.TargetType.TargetTypeId == (int)TargetType.Monthly));
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
            var actual = _periodService.GetPeriodById(periodId);

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
            var actual = _periodService.GetPeriodById(periodId);

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
            var actual = _periodService.GetPeriodById(periodId);

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
            var actual = _periodService.GetPeriodById(periodId);

            // Assert
            Assert.IsTrue(actual.TargetType.TargetTypeId != (int)TargetType.Monthly);
        }

        #endregion GetPeriodById

        #region AutoCreateMonthlyPeriod

        [Test]
        public void AutoCreateMonthlyPeriod_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeAutoCreateMonthlyPeriod();

            // Act
            var actual = _periodService.AutoCreateMonthlyPeriod(new AutoCreateMonthlyPeriodCriteria());

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
            _periodService.AutoCreateMonthlyPeriod(criteria);

            // Assert
            _periodService.Repository.Received().ExecuteNonQuery(Arg.Is<AutoCreateMonthlyPeriodCriteria>(x => x.IsValid()));
        }

        #endregion AutoCreateMonthlyPeriod

        #region GetPeriodEmployeeSettingById

        [Test]
        public void GetPeriodEmployeeSettingById_AnyArg_ReturnInstanceOfListOfPeriodEmployeeSettingModel()
        {
            // Arrange
            ArrangeGetPeriodEmployeeSettingById();

            // Act
            var actual = _periodService.GetPeriodEmployeeSettingById(Arg.Any<GetPeriodEmployeeSettingCriteria>());

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
            var actual = _periodService.GetPeriodEmployeeSettingById(criteria);

            // Assert
            Assert.AreEqual(result, actual);
        }

        #endregion GetPeriodEmployeeSettingById

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
            var actual = _periodService.UpdateEmployeeSettingOfPeriod(criteria);

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
            var actual = _periodService.UpdateEmployeeSettingOfPeriod(criteria);

            // Assert
            Assert.AreEqual(commandResult.Code, actual.Code);
            _manualMailService.DidNotReceive().SendNotifyMail(Arg.Any<PeriodModel>(), Arg.Any<List<PeriodFlowModel>>());
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
            var actual = _periodService.UpdateEmployeeSettingOfPeriod(criteria);

            // Assert
            Assert.AreEqual(commandResult.Code, 0);
            // _manualMailService.Received().SendNotifyMail(Arg.Any<PeriodModel>(), Arg.Any<List<PeriodFlowModel>>());
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
            var actual = _periodService.UpdateEmployeeSettingOfPeriod(criteria);

            // Assert
            Assert.AreEqual(commandResult.Code, actual.Code);
            _manualMailService.DidNotReceive().SendNotifyMail(Arg.Any<PeriodModel>(), Arg.Any<List<PeriodFlowModel>>());
        }

        #endregion UpdateEmployeeSettingOfPeriod

        #region GenerateEvaluation

        [Test]
        public void GenerateEvaluation_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeGenerateEvaluation();

            // Act
            var actual = _periodService.GenerateEvaluation(new AutoGenerateEvaluationCriteria());

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
            _periodService.GenerateEvaluation(criteria);

            // Assert
            _periodService.Repository.Received().ExecuteNonQuery(Arg.Is<AutoGenerateEvaluationCriteria>(x => x.IsValid()));
        }

        #endregion GenerateEvaluation

        #region GetEmployeeSettingByPeriodId

        [Test]
        public void GetEmployeeSettingByPeriodId_AnyArg_ReturnInstanceOfListOfPeriodFlowModel()
        {
            // Arrange
            ArrangeGetEmployeeSettingByPeriodId();

            // Act
            var actual = _periodService.GetEmployeeSettingByPeriodId(Arg.Any<GetEmployeeSettingByPeriodIdCriteria>());

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
            var actual = _periodService.GetProcessSettingByPeriodId(periodId);

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
            var actual = _periodService.GetProcessSettingByPeriodId(periodId);

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
            var actual = _periodService.GetEvaluationGlobalSettingByTargetTypeId(targetTypeId);

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
            var actual = _periodService.GetEvaluationGlobalSettingByTargetTypeId(targetTypeId);

            // Assert
            Assert.AreEqual(result, actual);
        }

        #endregion GetEvaluationGlobalSettingByTargetTypeId

        #region CheckSelfProcessStarted

        [Test]
        public void CheckSelfProcessStarted_AnyArg_ReturnIntanceOfBoolean()
        {
            // Arrange
            ArrangeCheckSelfProcessStarted();

            // Act
            var actual = _periodService.CheckSelfProcessStarted(Arg.Any<CheckSelfProcessStartedCriteria>());

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
            var actual = _periodService.CheckSelfProcessStarted(criteria);

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

        private IPeriodMailService FakeManualMailService()
        {
            IPeriodMailService service = Substitute.For<IPeriodMailService>();

            return service;
        }

        private PeriodService GetPeriodService(
            IBaseRepository baseRepository = null,
            IPeriodRepository periodRepository = null,
            IPeriodMailService manualMailService = null)
        {
            var service = new PeriodService(periodRepository, manualMailService);
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
                    TargetType = new TargetTypeModel
                    {
                        TargetTypeId = 1,
                        Name = "Probation"
                    }
                },
                new PeriodModel
                {
                    PeriodId = 2,
                    Name = "test 2",
                    TargetType = new TargetTypeModel
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
            _periodRepository = FakePeriodRepository();
            _periodRepository.GetPeriod(Arg.Any<GetPeriodCriteria>()).Returns(result ?? new List<PeriodModel>());
            _periodService = GetPeriodService(null, _periodRepository);
        }

        public void ArrangeGetPeriodById(PeriodModel result = null)
        {
            _periodRepository = FakePeriodRepository();
            _periodRepository.GetPeriodById(Arg.Any<GetPeriodByIdCriteria>()).Returns(result ?? new PeriodModel());
            _periodService = GetPeriodService(null, _periodRepository);
        }

        private void ArrangeAutoCreateMonthlyPeriod(CommandResult result = null)
        {
            _baseRepository = FakeBaseRepository();
            _periodService = GetPeriodService(_baseRepository);
            _periodService.Repository.ExecuteNonQuery(Arg.Any<AutoCreateMonthlyPeriodCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeGetPeriodEmployeeSettingById(IEnumerable<PeriodEmployeeSettingModel> result = null)
        {
            _periodRepository = FakePeriodRepository();
            _periodRepository.GetPeriodEmployeeSettingById(Arg.Any<GetPeriodEmployeeSettingCriteria>()).Returns(result ?? new List<PeriodEmployeeSettingModel>());
            _periodService = GetPeriodService(null, _periodRepository);
        }

        private void ArrangeGenerateEvaluation(CommandResult result = null)
        {
            _baseRepository = FakeBaseRepository();
            _periodService = GetPeriodService(_baseRepository);
            _periodService.Repository.ExecuteNonQuery(Arg.Any<AutoGenerateEvaluationCriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeGetEmployeeSettingByPeriodId(IEnumerable<PeriodFlowModel> result = null)
        {
            _periodRepository = FakePeriodRepository();
            _periodRepository.GetEmployeeSettingByPeriodId(Arg.Any<GetEmployeeSettingByPeriodIdCriteria>()).Returns(result ?? new List<PeriodFlowModel>());
            _periodService = GetPeriodService(null, _periodRepository);
        }

        private void ArrangeGetProcessSettingByPeriodId(IEnumerable<FlowModel> result = null)
        {
            _periodService = Substitute.For<PeriodService>();
            _periodService.GetData<FlowModel>(Arg.Any<GetProcessSettingPeriodCriteria>()).Returns(result ?? new List<FlowModel>());
        }

        private void ArrangeGetEvaluationGlobalSettingByTargetTypeId(IEnumerable<FlowModel> result = null)
        {
            _periodService = Substitute.For<PeriodService>();
            _periodService.GetData<FlowModel>(Arg.Any<GetEvaluationGlobalSettingByTargetTypeIdCriteria>()).Returns(result ?? new List<FlowModel>());
        }

        private void ArrangeCheckSelfProcessStarted(bool result = false)
        {
            _periodService = Substitute.For<PeriodService>();
            _periodService.GetDataEntity<bool>(Arg.Any<CheckSelfProcessStartedCriteria>()).Returns(result);
        }

        private void ArrangeUpdateEmployeeSettingOfPeriod(CommandResult result = null, string appSetting = null)
        {
            var period = new PeriodModel();
            var employeeSettings = new List<PeriodFlowModel>();

            _baseRepository = FakeBaseRepository();
            _manualMailService = FakeManualMailService();
            _periodRepository = FakePeriodRepository();

            _periodService = GetPeriodService(_baseRepository, _periodRepository, _manualMailService);
            _periodService.Repository.ExecuteNonQuery(Arg.Any<UpdateEmployeeSettingOfPeriodCriteria>()).Returns(result ?? new CommandResult());
            _periodRepository.GetPeriodById(Arg.Any<GetPeriodByIdCriteria>()).Returns(period);
            _periodRepository.GetEmployeeSettingByPeriodId(Arg.Any<GetEmployeeSettingByPeriodIdCriteria>()).Returns(employeeSettings);
        }

        #endregion Helpers
    }
}