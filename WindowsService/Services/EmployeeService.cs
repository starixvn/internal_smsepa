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

    public class EmployeeService : IEmployeeService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IEmployeeAPI employeeApi;

        public EmployeeService(IDynamicRepository dynamicRepository, IEmployeeAPI employeeApi)
        {
            this.dynamicRepository = dynamicRepository;
            this.employeeApi = employeeApi;
        }

        public IEnumerable<Employee> GetEmployees(DateTime? fromDate)
        {
            var criteria = new GetEmployeeCriteria { FromDate = fromDate };
            IEnumerable<Employee> employees = dynamicRepository.Fetch<Employee>(criteria);

            return employees;
        }

        public async Task<DateTime> GetLastModifiedDateAsync(TokenAccess token)
        {
            return await employeeApi.GetLastModifiedDateAsync(token);
        }

        public async Task<CommandResult> UpdateAsync(IEnumerable<Employee> employees, TokenAccess token)
        {
            return await employeeApi.Update(employees, token);
        }
    }
}