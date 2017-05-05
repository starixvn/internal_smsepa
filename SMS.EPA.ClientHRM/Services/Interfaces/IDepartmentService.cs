namespace SMS.EPA.ClientHrm.Services
{
    using Core.Model;
    using Model;
    using System;
    using System.Collections.Generic;

    public interface IDepartmentService
    {
        IEnumerable<Department> GetDepartments(DateTime? fromDate);

        DateTime GetLastModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<Department> departments, TokenAccess token);
    }
}