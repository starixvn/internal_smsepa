namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Repositories;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model;
    using SMS.EPA.Repositories;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    [TestFixture]
    public class PeriodServiceTests
    {
        private PeriodService periodService;
        private IManualMailService manualMailService;
        private IConfigurationManagerService configurationManager;
        private IBaseRepository baseRepository;
        private IPeriodRepository periodRepository;

        #region GetPeriod

        [Test]
        public void GetPeriod_AnyArg()
        {
        }

        #endregion GetPeriod

        #region Helpers

        private IBaseRepository FakeBaseRepository()
        {
            IBaseRepository repository = Substitute.For<IBaseRepository>();

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

        private void ArrangeGetPeriod(IEnumerable<PeriodModel> result = null)
        {
        }

        #endregion Helpers
    }
}