namespace SMS.EPA.ClientHrm.Services
{
    using Core.Model;
    using EPA.Model.API;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IEmployeeService
    {
        IEnumerable<Employee> GetEmployees(DateTime? fromDate);

        Task<DateTime> GetLastModifiedDateAsync(TokenAccess token);

        Task<CommandResult> UpdateAsync(IEnumerable<Employee> employees, TokenAccess token);
    }
}