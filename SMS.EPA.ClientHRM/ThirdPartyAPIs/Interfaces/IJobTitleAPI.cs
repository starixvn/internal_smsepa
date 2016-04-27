using SMS.EPA.ClientHRM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.EPA.ClientHRM.ThirdPartyAPIs
{
    public interface IJobTitleAPI
    {
        CommandResult Update(IEnumerable<JobTitle> departments, TokenAccess token);
    }
}
