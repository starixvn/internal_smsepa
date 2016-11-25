namespace SMS.EPA.ClientHrm.Services
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.ClientHrm.Model;

    public interface IEmployeeService
    {
        IEnumerable<Employee> GetEmployees(DateTime? fromDate);

        DateTime GetLastModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<Employee> employees, TokenAccess token);
    }
}