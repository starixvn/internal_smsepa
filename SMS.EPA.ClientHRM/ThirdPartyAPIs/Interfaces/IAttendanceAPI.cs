using SMS.EPA.ClientHRM.Model;
using SMS.EPA.ServicesAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.EPA.ClientHRM.ThirdPartyAPIs
{
    public interface IAttendanceAPI
    {
        DateTime GetMaxModifiedDate(TokenAccess token);

        DateTime GetFromDateAttendanceViolation(TokenAccess token);

        CommandResult Update(IEnumerable<AttendanceViolation> violations, TokenAccess token);
    }
}
