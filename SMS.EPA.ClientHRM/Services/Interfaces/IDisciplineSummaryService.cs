namespace SMS.EPA.ClientHrm.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Model;
    using EPA.Model.API;
    using Model;

    public interface IDisciplineSummaryService
    {
        IEnumerable<DisciplineSummary> GetDisciplineSummary(DateTime lastModifiedDate);

        Task<DateTime> GetLastModifiedDateAsync(TokenAccess token);

        Task<CommandResult> UpdateAsync(IEnumerable<DisciplineSummary> disciplines, TokenAccess token);
    }
}