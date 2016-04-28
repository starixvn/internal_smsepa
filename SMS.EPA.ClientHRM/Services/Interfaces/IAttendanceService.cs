namespace SMS.EPA.ClientHRM.Services
{
    using SMS.EPA.ClientHRM.Model;
    using SMS.EPA.ServicesAPI.Model;
    using System;
    using System.Collections.Generic;

    public interface IAttendanceService
    {
        IEnumerable<AttendanceViolation> GetViolation(DateTime fromDate, DateTime lastModifiedDate);

        DateTime GetMaxModifiedDate(TokenAccess token);

        DateTime GetFromDateAttendanceViolation(TokenAccess token);

        CommandResult Update(IEnumerable<AttendanceViolation> violations, TokenAccess token);
    }
}
