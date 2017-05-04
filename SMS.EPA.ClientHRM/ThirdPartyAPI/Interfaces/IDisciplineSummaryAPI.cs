namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using EPA.Model;
    using Model;
    using System;
    using System.Collections.Generic;

    public interface IDisciplineSummaryAPI
    {
        DateTime GetLastModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<DisciplineSummary> disciplines, TokenAccess token);
    }
}