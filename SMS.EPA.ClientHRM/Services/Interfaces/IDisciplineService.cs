namespace SMS.EPA.ClientHRM.Services
{
    using SMS.EPA.ClientHRM.Model;
    using SMS.EPA.ServicesAPI.Model;
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
