namespace SMS.EPA.ClientHRM.ThirdPartyAPIs
{
    using SMS.EPA.ClientHRM.Model;
    using System;
    using System.Collections.Generic;

    public interface ISupervisorAPI
    {
        CommandResult UpdateEmployees(IEnumerable<EmployeeSupervisor> employees, TokenAccess token);

        CommandResult UpdateGroups(IEnumerable<GroupSupervisor> groups, TokenAccess token);

        DateTime GetMaxModifiedDateForEmployee(TokenAccess token);

        DateTime GetMaxModifiedDateForGroup(TokenAccess token);
    }
}
