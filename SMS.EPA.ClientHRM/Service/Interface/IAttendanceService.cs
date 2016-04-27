using SMS.EPA.ClientHRM.Model;
using SMS.EPA.ServicesAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.EPA.ClientHRM.Service
{
    public interface IAttendanceService
    {
        IEnumerable<AttendanceViolation> GetViolation(DateTime fromDate, DateTime lastModifiedDate);

        DateTime GetMaxModifiedDate(TokenAccess token);

        DateTime GetFromDateAttendanceViolation(TokenAccess token);

        CommandResult Update(IEnumerable<AttendanceViolation> violations, TokenAccess token);
    }
}
