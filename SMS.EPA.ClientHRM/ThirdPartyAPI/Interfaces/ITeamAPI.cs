namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using Model;
    using System;
    using System.Collections.Generic;

    public interface ITeamAPI
    {
        DateTime GetLastModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<Team> teams, TokenAccess token);
    }
}