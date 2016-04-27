using SMS.EPA.ClientHRM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.EPA.ClientHRM.ThirdPartyAPIs
{
    public interface ISupervisorAPI
    {
        CommandResult UpdateEmployees(IEnumerable<EmployeeSupervisor> employees, TokenAccess token);

        CommandResult UpdateGroups(IEnumerable<GroupSupervisor> groups, TokenAccess token);

        DateTime GetMaxModifiedDateForEmployee(TokenAccess token);

        DateTime GetMaxModifiedDateForGroup(TokenAccess token);
    }
}
