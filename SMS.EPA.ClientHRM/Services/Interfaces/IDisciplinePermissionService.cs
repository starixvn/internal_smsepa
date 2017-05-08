namespace SMS.EPA.ClientHrm.Services
{
    using Core.Model;
    using Model;
    using System;
    using System.Collections.Generic;
    using EPA.Model.API;

    public interface ISupervisorService
    {
        IEnumerable<EmployeeDisciplinePermission> GetEmployees(DateTime fromDate);

        IEnumerable<TeamDisciplinePermission> GetGroups(DateTime fromDate);

        CommandResult UpdateEmployees(IEnumerable<EmployeeDisciplinePermission> employees, TokenAccess token);

        CommandResult UpdateGroups(IEnumerable<TeamDisciplinePermission> groups, TokenAccess token);

        DateTime GetLastModifiedDateForEmployee(TokenAccess token);

        DateTime GetLastModifiedDateForGroup(TokenAccess token);
    }
}