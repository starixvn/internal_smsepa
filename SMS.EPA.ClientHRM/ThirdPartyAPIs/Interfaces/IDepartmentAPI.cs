namespace SMS.EPA.ClientHRM.ThirdPartyAPIs
{
    using SMS.EPA.ClientHRM.Model;
    using System;
    using System.Collections.Generic;

    public interface IDepartmentAPI
    {
        DateTime GetLastModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<Department> departments, TokenAccess token);
    }
}
