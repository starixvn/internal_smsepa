namespace SMS.EPA.Services.UnitTests
{
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Repositories;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Implement;

    [TestFixture]
    public class FlowManagementServiceTests
    {
        private FlowSettingService flowManagementService;
        private IFlowSettingRepository flowManagementRepository;
        private IBaseRepository baseRepository;

        #region Helpers

        private IBaseRepository FakeBaseRepository()
        {
            var repository = Substitute.For<IBaseRepository>();

            return repository;
        }

        private IFlowSettingRepository FakeFlowManagementRepository()
        {
            var repository = Substitute.For<IFlowSettingRepository>();

            return repository;
        }

        private FlowSettingService GetFlowManagementService(
            IFlowSettingRepository flowManagementRepository = null,
            IBaseRepository baseRepository = null)
        {
            var service = new FlowSettingService(flowManagementRepository);
            service.Repository = baseRepository;

            return service;
        }

        #endregion Helpers
    }
}