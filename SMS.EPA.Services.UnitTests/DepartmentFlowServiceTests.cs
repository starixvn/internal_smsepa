namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Repositories;
    using SMS.EPA.Model;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Implement;

    [TestFixture]
    public class DepartmentFlowServiceTests
    {
        //// private DepartmentFlowService departmentFlowService;
        //// private IFlowSettingRepository flowSettingRepository;
        //// private IBaseRepository baseRepository;

        #region Helpers

        private IBaseRepository FakeBaseRepository()
        {
            var repository = Substitute.For<IBaseRepository>();

            return repository;
        }

        private IFlowSettingRepository FakeFlowSettingRepository()
        {
            var repository = Substitute.For<IFlowSettingRepository>();

            return repository;
        }

        private DepartmentFlowService GetDepartmentFlowService(
            IFlowSettingRepository flowSettingRepository,
            IBaseRepository baseRepository = null)
        {
            var service = new DepartmentFlowService(flowSettingRepository);
            service.Repository = baseRepository;

            return service;
        }

        private IEnumerable<DepartmentFlowModel> GetListOfDepartmentFlowForTest()
        {
            var flowTemplates = new List<DepartmentFlowModel>
            {
                new DepartmentFlowModel
                {
                    DepartmentId = 1,
                    Code = "HR",
                    Name = "HR",
                    HumanResources = new List<ProcessFlowModel>
                    {
                        new ProcessFlowModel { EmployeeId = 1, FullName = "a" }
                    }
                },
                new DepartmentFlowModel
                {
                    DepartmentId = 2,
                    Code = "Management",
                    Name = "Management"
                }
            };

            return flowTemplates;
        }

        #endregion Helpers
    }
}