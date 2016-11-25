namespace SMS.EPA.ClientHrm.ThirdPartyAPIs
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.ClientHrm.Model;

    public interface IDisciplineAPI
    {
        DateTime GetLastModifiedDate(TokenAccess token);

        DateTime GetFromDateDiscipline(TokenAccess token);

        CommandResult Update(IEnumerable<Discipline> disciplines, TokenAccess token);
    }
}