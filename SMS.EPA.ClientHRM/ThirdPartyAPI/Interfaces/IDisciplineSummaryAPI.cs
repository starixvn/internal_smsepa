namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.ClientHrm.Model;

    public interface IDisciplineSummaryAPI
    {
        DateTime GetLastModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<DisciplineSummary> disciplines, TokenAccess token);
    }
}