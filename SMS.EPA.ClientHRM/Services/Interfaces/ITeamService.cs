namespace SMS.EPA.ClientHrm.Services
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.ClientHrm.Model;

    public interface ITeamService
    {
        IEnumerable<Team> GetTeams(DateTime? fromDate);

        DateTime GetLastModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<Team> teams, TokenAccess token);
    }
}