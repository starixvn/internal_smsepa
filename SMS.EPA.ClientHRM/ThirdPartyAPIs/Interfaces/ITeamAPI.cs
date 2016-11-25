namespace SMS.EPA.ClientHrm.ThirdPartyAPIs
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.ClientHrm.Model;

    public interface ITeamAPI
    {
        DateTime GetLastModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<Team> departments, TokenAccess token);
    }
}