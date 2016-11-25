namespace SMS.EPA.ClientHrm.ThirdPartyAPIs
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.ClientHrm.Model;

    public interface IEmployeeSupervisorAPI
    {
        CommandResult Update(IEnumerable<EmployeeDisciplinePermission> employees, TokenAccess token);

        DateTime GetLastModifiedDate(TokenAccess token);
    }
}