namespace SMS.EPA.WindowsService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Model;
    using EPA.Model.API;
    using Model;

    public interface ISupervisorService
    {
        IEnumerable<EmployeeDisciplinePermission> GetEmployees(DateTime fromDate);

        IEnumerable<TeamDisciplinePermission> GetGroups(DateTime fromDate);

        Task<CommandResult> UpdateEmployeesAsync(IEnumerable<EmployeeDisciplinePermission> employees, TokenAccess token);

        Task<CommandResult> UpdateGroupsAsync(IEnumerable<TeamDisciplinePermission> groups, TokenAccess token);

        Task<DateTime> GetLastModifiedDateForEmployeeAsync(TokenAccess token);

        Task<DateTime> GetLastModifiedDateForGroup(TokenAccess token);
    }
}