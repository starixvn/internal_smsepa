using SMS.EPA.ClientHRM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.EPA.ClientHRM.ThirdPartyAPIs
{
    public interface IEmployeeAPI
    {
        DateTime GetMaxModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<Employee> departments, TokenAccess token);
    }
}
