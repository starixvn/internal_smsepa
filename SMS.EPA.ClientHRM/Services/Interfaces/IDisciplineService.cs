namespace SMS.EPA.ClientHrm.Services
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.ClientHrm.Model;

    public interface IDisciplineService
    {
        IEnumerable<Discipline> GetDiscipline(DateTime fromDate, DateTime lastModifiedDate);

        DateTime GetLastModifiedDate(TokenAccess token);

        DateTime GetFromDateDiscipline(TokenAccess token);

        CommandResult Update(IEnumerable<Discipline> disciplines, TokenAccess token);
    }
}