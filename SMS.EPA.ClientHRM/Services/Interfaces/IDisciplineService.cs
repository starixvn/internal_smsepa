namespace SMS.EPA.ClientHrm.Services
{
    using Model;
    using System;
    using System.Collections.Generic;

    public interface IDisciplineService
    {
        IEnumerable<Discipline> GetDiscipline(DateTime fromDate, DateTime lastModifiedDate);

        DateTime GetLastModifiedDate(TokenAccess token);

        DateTime GetFromDateDiscipline(TokenAccess token);

        CommandResult Update(IEnumerable<Discipline> disciplines, TokenAccess token);
    }
}