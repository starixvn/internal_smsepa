namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using Model;
    using System;
    using System.Collections.Generic;

    public interface IDisciplineAPI
    {
        DateTime GetLastModifiedDate(TokenAccess token);

        DateTime GetFromDateDiscipline(TokenAccess token);

        CommandResult Update(IEnumerable<Discipline> disciplines, TokenAccess token);
    }
}