namespace SMS.EPA.ClientHRM.Services
{
    using SMS.EPA.ClientHRM.Model;
    using System;
    using System.Collections.Generic;

    public interface ISupervisorService
    {
        IEnumerable<EmployeeSupervisor> GetEmployees(DateTime fromDate);

        IEnumerable<GroupSupervisor> GetGroups(DateTime fromDate);

        CommandResult UpdateEmployees(IEnumerable<EmployeeSupervisor> employees, TokenAccess token);

        CommandResult UpdateGroups(IEnumerable<GroupSupervisor> groups, TokenAccess token);

        DateTime GetMaxModifiedDateForEmployee(TokenAccess token);

        DateTime GetMaxModifiedDateForGroup(TokenAccess token);
    }
}
