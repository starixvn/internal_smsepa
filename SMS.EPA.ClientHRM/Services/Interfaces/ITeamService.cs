namespace SMS.EPA.ClientHRM.Services
{
    using SMS.EPA.ClientHRM.Model;
    using System;
    using System.Collections.Generic;

    public interface ITeamService
    {
        IEnumerable<Team> GetTeams(DateTime? fromDate);

        DateTime GetLastModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<Team> teams, TokenAccess token);
    }
}
