namespace SMS.EPA.ClientHrm.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.ClientHrm.Criteria;
    using SMS.EPA.ClientHrm.Model;
    using SMS.EPA.ClientHrm.ThirdPartyAPIs;

    public class DepartmentService : IDepartmentService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IDepartmentAPI departmentAPI;

        public DepartmentService(IDynamicRepository dynamicRepository, IDepartmentAPI departmentAPI)
        {
            this.dynamicRepository = dynamicRepository;
            this.departmentAPI = departmentAPI;
        }

        public IEnumerable<Department> GetDepartments(DateTime? fromDate)
        {
            var criteria = new GetDepartmentCriteria { FromDate = fromDate };
            IEnumerable<Department> departments = dynamicRepository.Fetch<Department>(criteria);

            return departments;
        }

        public DateTime GetLastModifiedDate(TokenAccess token)
        {
            return departmentAPI.GetLastModifiedDate(token);
        }

        public CommandResult Update(IEnumerable<Department> departments, TokenAccess token)
        {
            return departmentAPI.Update(departments, token);
        }
    }
}