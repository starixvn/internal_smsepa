namespace SMS.EPA.ClientHrm.ThirdPartyAPIs
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.ClientHrm.Model;

    public interface IEmployeeAPI
    {
        DateTime GetLastModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<Employee> departments, TokenAccess token);
    }
}