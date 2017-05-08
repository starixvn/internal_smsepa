namespace SMS.EPA.ClientHrm.Services
{
    using Core.Model;
    using Criteria;
    using EPA.Model.API;
    using Fanex.Data.Repository;
    using Model;
    using System;
    using System.Collections.Generic;
    using ThirdPartyAPI;

    public class EmployeeService : IEmployeeService
    {
        private readonly IDynamicRepository _dynamicRepository;
        private readonly IEmployeeAPI _employeeApi;

        public EmployeeService(IDynamicRepository dynamicRepository, IEmployeeAPI employeeApi)
        {
            _dynamicRepository = dynamicRepository;
            _employeeApi = employeeApi;
        }

        public IEnumerable<Employee> GetEmployees(DateTime? fromDate)
        {
            var criteria = new GetEmployeeCriteria { FromDate = fromDate };
            IEnumerable<Employee> employees = _dynamicRepository.Fetch<Employee>(criteria);

            return employees;
        }

        public DateTime GetLastModifiedDate(TokenAccess token)
        {
            return _employeeApi.GetLastModifiedDate(token);
        }

        public CommandResult Update(IEnumerable<Employee> employees, TokenAccess token)
        {
            return _employeeApi.Update(employees, token);
        }
    }
}