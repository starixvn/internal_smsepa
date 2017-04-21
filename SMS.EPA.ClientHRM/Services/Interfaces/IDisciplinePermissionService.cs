namespace SMS.EPA.ClientHrm.Services
{
    using Model;
    using System;
    using System.Collections.Generic;

    public interface ISupervisorService
    {
        IEnumerable<EmployeeDisciplinePermission> GetEmployees(DateTime fromDate);

        IEnumerable<GroupDisciplinePermission> GetGroups(DateTime fromDate);

        CommandResult UpdateEmployees(IEnumerable<EmployeeDisciplinePermission> employees, TokenAccess token);

        CommandResult UpdateGroups(IEnumerable<GroupDisciplinePermission> groups, TokenAccess token);

        DateTime GetLastModifiedDateForEmployee(TokenAccess token);

        DateTime GetLastModifiedDateForGroup(TokenAccess token);
    }
}