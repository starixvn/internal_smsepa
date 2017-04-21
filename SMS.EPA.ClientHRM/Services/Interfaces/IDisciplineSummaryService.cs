namespace SMS.EPA.ClientHrm.Services
{
    using Model;
    using System;
    using System.Collections.Generic;

    public interface IDisciplineSummaryService
    {
        IEnumerable<DisciplineSummary> GetDisciplineSummary(DateTime lastModifiedDate);

        DateTime GetLastModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<DisciplineSummary> disciplines, TokenAccess token);
    }
}