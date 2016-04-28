namespace SMS.EPA.ClientHRM.ThirdPartyAPIs
{
    using SMS.EPA.ClientHRM.Model;
    using System;
    using System.Collections.Generic;

    public interface ITeamAPI
    {
        DateTime GetMaxModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<Team> departments, TokenAccess token);
    }
}
