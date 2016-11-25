namespace SMS.EPA.ClientHrm.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.ClientHrm.Criteria;
    using SMS.EPA.ClientHrm.Model;
    using SMS.EPA.ClientHrm.ThirdPartyAPIs;

    public class EmployeeService : IEmployeeService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IEmployeeAPI employeeAPI;

        public EmployeeService(IDynamicRepository dynamicRepository, IEmployeeAPI employeeAPI)
        {
            this.dynamicRepository = dynamicRepository;
            this.employeeAPI = employeeAPI;
        }

        public IEnumerable<Employee> GetEmployees(DateTime? fromDate)
        {
            var criteria = new GetEmployeeCriteria { FromDate = fromDate };
            IEnumerable<Employee> employees = dynamicRepository.Fetch<Employee>(criteria);

            return employees;
        }

        public DateTime GetLastModifiedDate(TokenAccess token)
        {
            return employeeAPI.GetLastModifiedDate(token);
        }

        public CommandResult Update(IEnumerable<Employee> employees, TokenAccess token)
        {
            return employeeAPI.Update(employees, token);
        }
    }
}