namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.ClientHrm.Model;

    public interface IEmployeeSupervisorAPI
    {
        CommandResult Update(IEnumerable<EmployeeDisciplinePermission> employeeSupervisors, TokenAccess token);

        DateTime GetLastModifiedDate(TokenAccess token);
    }
}