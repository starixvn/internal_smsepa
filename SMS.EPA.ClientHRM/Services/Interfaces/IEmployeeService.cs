namespace SMS.EPA.ClientHrm.Services
{
    using Model;
    using System;
    using System.Collections.Generic;

    public interface IEmployeeService
    {
        IEnumerable<Employee> GetEmployees(DateTime? fromDate);

        DateTime GetLastModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<Employee> employees, TokenAccess token);
    }
}