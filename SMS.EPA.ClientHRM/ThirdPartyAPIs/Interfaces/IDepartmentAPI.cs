namespace SMS.EPA.ClientHrm.ThirdPartyAPIs
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.ClientHrm.Model;

    public interface IDepartmentAPI
    {
        DateTime GetLastModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<Department> departments, TokenAccess token);
    }
}