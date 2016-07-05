namespace SMS.EPA.ClientHRM.ThirdPartyAPIs
{
    using SMS.EPA.ClientHRM.Model;
    using System;
    using System.Collections.Generic;

    public interface IEmployeeSupervisorAPI
    {
        CommandResult Update(IEnumerable<EmployeeDisciplinePermission> employees, TokenAccess token);

        DateTime GetMaxModifiedDate(TokenAccess token);
    }
}
