using SMS.EPA.ClientHRM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.EPA.ClientHRM.ThirdPartyAPI
{
    public interface IDepartmentAPI
    {
        DateTime GetMaxModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<Department> departments, TokenAccess token);
    }
}
