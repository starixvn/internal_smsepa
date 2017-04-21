namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using Model;
    using System;
    using System.Collections.Generic;

    public interface IEmployeeSupervisorAPI
    {
        CommandResult Update(IEnumerable<EmployeeDisciplinePermission> employeeSupervisors, TokenAccess token);

        DateTime GetLastModifiedDate(TokenAccess token);
    }
}