namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using Model;
    using System;
    using System.Collections.Generic;

    public interface IDepartmentAPI
    {
        DateTime GetLastModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<Department> departments, TokenAccess token);
    }
}