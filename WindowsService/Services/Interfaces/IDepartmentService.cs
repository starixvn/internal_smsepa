namespace SMS.EPA.WindowsService.Services
{
    using Core.Model;
    using EPA.Model.API;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDepartmentService
    {
        IEnumerable<Department> GetDepartments(DateTime? fromDate);

        Task<DateTime> GetLastModifiedDateAsync(TokenAccess token);

        Task<CommandResult> Update(IEnumerable<Department> departments, TokenAccess token);
    }
}