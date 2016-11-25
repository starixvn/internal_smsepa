namespace SMS.EPA.ClientHrm.Services
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.ClientHrm.Model;

    public interface IDepartmentService
    {
        IEnumerable<Department> GetDepartments(DateTime? fromDate);

        DateTime GetLastModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<Department> departments, TokenAccess token);
    }
}