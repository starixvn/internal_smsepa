namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using Core.Model;
    using EPA.Model.API;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDisciplineSummaryAPI
    {
        Task<DateTime> GetLastModifiedDateAsync(TokenAccess token);

        Task<CommandResult> UpdateAsync(IEnumerable<DisciplineSummary> disciplines, TokenAccess token);
    }
}