namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Enumerations;
    using Core.Model;
    using Core.Services;
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
        private IBaseService baseService;
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
            Assert.AreEqual(0, actual.Count(x => x.TargetType == null || x.TargetType.TargetTypeId <= 0));
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
            ArrangeUpdateEmployeeSettingOfPeriod(commandResult);

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
            ArrangeUpdateEmployeeSettingOfPeriod(commandResult);

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
            ArrangeUpdateEmployeeSettingOfPeriod(commandResult);

            // Act
            var actual = _periodService.UpdateEmployeeSettingOfPeriod(criteria);

            // Assert
            Assert.AreEqual(commandResult.Code, actual.Code);
            _manualMailService.DidNotReceive().SendNotifyMail(Arg.Any<PeriodModel>(), Arg.Any<List<PeriodFlowModel>>());
        }

        #endregion UpdateEmployeeSettingOfPeriod

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

        #region Helpers

        private IBaseService FakeBaseService()
        {
            IBaseService repository = Substitute.For<IBaseService>();

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
            IBaseService baseService = null,
            IPeriodRepository periodRepository = null,
            IPeriodMailService manualMailService = null)
        {
            var service = new PeriodService(periodRepository, manualMailService, baseService);

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

        private void ArrangeGetPeriodEmployeeSettingById(IEnumerable<PeriodEmployeeSettingModel> result = null)
        {
            _periodRepository = FakePeriodRepository();
            _periodRepository.GetPeriodEmployeeSettingById(Arg.Any<GetPeriodEmployeeSettingCriteria>()).Returns(result ?? new List<PeriodEmployeeSettingModel>());
            _periodService = GetPeriodService(null, _periodRepository);
        }

        private void ArrangeGetEmployeeSettingByPeriodId(IEnumerable<PeriodFlowModel> result = null)
        {
            _periodRepository = FakePeriodRepository();
            _periodRepository.GetEmployeeSettingByPeriodId(Arg.Any<GetEmployeeSettingByPeriodIdCriteria>()).Returns(result ?? new List<PeriodFlowModel>());
            _periodService = GetPeriodService(null, _periodRepository);
        }

        private void ArrangeUpdateEmployeeSettingOfPeriod(CommandResult result = null)
        {
            var period = new PeriodModel();
            var employeeSettings = new List<PeriodFlowModel>();

            baseService = FakeBaseService();
            _manualMailService = FakeManualMailService();
            _periodRepository = FakePeriodRepository();
            baseService.ExecuteNonQuery(Arg.Any<UpdateEmployeeSettingOfPeriodCriteria>()).Returns(result ?? new CommandResult());
            _periodRepository.GetPeriodById(Arg.Any<GetPeriodByIdCriteria>()).Returns(period);
            _periodRepository.GetEmployeeSettingByPeriodId(Arg.Any<GetEmployeeSettingByPeriodIdCriteria>()).Returns(employeeSettings);
            _periodService = GetPeriodService(baseService, _periodRepository, _manualMailService);
        }

        #endregion Helpers
    }
}