namespace SMS.EPA.ClientHRM.Services
{
    using SMS.EPA.ClientHRM.Model;
    using System;
    using System.Collections.Generic;

    public interface IEmployeeService
    {
        IEnumerable<Employee> GetEmployees(DateTime? fromDate);

        DateTime GetLastModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<Employee> departments, TokenAccess token);
    }
}
