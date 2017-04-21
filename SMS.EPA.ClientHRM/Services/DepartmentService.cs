namespace SMS.EPA.ClientHrm.Services
{
    using Criteria;
    using Fanex.Data.Repository;
    using Model;
    using System;
    using System.Collections.Generic;
    using ThirdPartyAPI;

    public class DepartmentService : IDepartmentService
    {
        private readonly IDynamicRepository _dynamicRepository;
        private readonly IDepartmentAPI _departmentApi;

        public DepartmentService(IDynamicRepository dynamicRepository, IDepartmentAPI departmentApi)
        {
            _dynamicRepository = dynamicRepository;
            _departmentApi = departmentApi;
        }

        public IEnumerable<Department> GetDepartments(DateTime? fromDate)
        {
            var criteria = new GetDepartmentCriteria { FromDate = fromDate };
            IEnumerable<Department> departments = _dynamicRepository.Fetch<Department>(criteria);

            return departments;
        }

        public DateTime GetLastModifiedDate(TokenAccess token)
        {
            return _departmentApi.GetLastModifiedDate(token);
        }

        public CommandResult Update(IEnumerable<Department> departments, TokenAccess token)
        {
            return _departmentApi.Update(departments, token);
        }
    }
}