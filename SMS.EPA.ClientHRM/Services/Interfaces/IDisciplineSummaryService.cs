namespace SMS.EPA.ClientHrm.Services
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.ClientHrm.Model;

    public interface IDisciplineSummaryService
    {
        IEnumerable<DisciplineSummary> GetDisciplineSummary(DateTime lastModifiedDate);

        DateTime GetLastModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<DisciplineSummary> disciplines, TokenAccess token);
    }
}