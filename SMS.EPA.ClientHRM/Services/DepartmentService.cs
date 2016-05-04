namespace SMS.EPA.ClientHRM.Services
{
    using Fanex.Data.Repository;
    using SMS.EPA.ClientHRM.Criteria;
    using SMS.EPA.ClientHRM.Model;
    using SMS.EPA.ClientHRM.ThirdPartyAPIs;
    using System;
    using System.Collections.Generic;

    public class DepartmentService : IDepartmentService
    {
        private readonly IDynamicRepository _dynamicRepository;
        private readonly IDepartmentAPI _departmentAPI;

        public DepartmentService(IDynamicRepository dynamicRepository, IDepartmentAPI departmentAPI)
        {
            _dynamicRepository = dynamicRepository;
            _departmentAPI = departmentAPI;
        }

        public IEnumerable<Department> GetDepartments(DateTime? fromDate)
        {
            var criteria = new GetDepartmentCriteria { FromDate = fromDate };
            IEnumerable<Department> departments = _dynamicRepository.Fetch<Department>(criteria);

            return departments;
        }

        public DateTime GetMaxModifiedDate(TokenAccess token)
        {
            return _departmentAPI.GetMaxModifiedDate(token);
        }

        public CommandResult Update(IEnumerable<Department> departments, TokenAccess token)
        {
            return _departmentAPI.Update(departments, token);
        }
    }
}
