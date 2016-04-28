namespace SMS.EPA.ClientHRM.Services
{
    using Fanex.Data.Repository;
    using SMS.EPA.ClientHRM.Criteria;
    using SMS.EPA.ClientHRM.Model;
    using SMS.EPA.ClientHRM.ThirdPartyAPIs;
    using System;
    using System.Collections.Generic;

    public class EmployeeService : IEmployeeService
    {
        private readonly IDynamicRepository _dynamicRepository;
        private readonly IEmployeeAPI _employeeAPI;

        public EmployeeService() : this(null, null)
        {
        }

        public EmployeeService(IDynamicRepository dynamicRepository, IEmployeeAPI employeeAPI)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
            _employeeAPI = employeeAPI ?? new EmployeeAPI();
        }

        public IEnumerable<Employee> GetEmployees(DateTime? fromDate)
        {
            var criteria = new GetEmployeeCriteria { FromDate = fromDate };
            IEnumerable<Employee> employees = _dynamicRepository.Fetch<Employee>(criteria);

            return employees;
        }

        public DateTime GetMaxModifiedDate(TokenAccess token)
        {
            return _employeeAPI.GetMaxModifiedDate(token);
        }

        public CommandResult Update(IEnumerable<Employee> employees, TokenAccess token)
        {
            return _employeeAPI.Update(employees, token);
        }
    }
}
