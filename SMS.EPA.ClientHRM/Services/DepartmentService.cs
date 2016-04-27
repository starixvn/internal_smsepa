using Fanex.Data.Repository;
using SMS.EPA.ClientHRM.Criteria;
using SMS.EPA.ClientHRM.Model;
using SMS.EPA.ClientHRM.ThirdPartyAPIs;
using System;
using System.Collections.Generic;

namespace SMS.EPA.ClientHRM.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDynamicRepository _dynamicRepository;
        private readonly IDepartmentAPI _departmentAPI;

        public DepartmentService() : this(null, null)
        { 
        }

        public DepartmentService(IDynamicRepository dynamicRepository, IDepartmentAPI departmentAPI)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
            _departmentAPI = departmentAPI ?? new DepartmentAPI();
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
