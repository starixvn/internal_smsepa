namespace SMS.EPA.WindowsService.Services
{
    using Core.Model;
    using Criteria;
    using EPA.Model.API;
    using Fanex.Data.Repository;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ThirdPartyAPI;

    public class DepartmentService : IDepartmentService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IDepartmentAPI departmentApi;

        public DepartmentService(IDynamicRepository dynamicRepository, IDepartmentAPI departmentApi)
        {
            this.dynamicRepository = dynamicRepository;
            this.departmentApi = departmentApi;
        }

        public IEnumerable<Department> GetDepartments(DateTime? fromDate)
        {
            var criteria = new GetDepartmentCriteria { FromDate = fromDate };
            IEnumerable<Department> departments = dynamicRepository.Fetch<Department>(criteria);

            return departments;
        }

        public async Task<DateTime> GetLastModifiedDateAsync(TokenAccess token)
        {
            return await departmentApi.GetLastModifiedDate(token);
        }

        public Task<CommandResult> Update(IEnumerable<Department> departments, TokenAccess token)
        {
            return departmentApi.UpdateAsync(departments, token);
        }
    }
}