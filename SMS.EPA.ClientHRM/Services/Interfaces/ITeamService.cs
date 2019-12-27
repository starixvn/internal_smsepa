namespace SMS.EPA.ClientHrm.Services
{
    using Core.Model;
    using EPA.Model.API;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITeamService
    {
        IEnumerable<Team> GetTeams(DateTime? fromDate);

        Task<DateTime> GetLastModifiedDateAsync(TokenAccess token);

        Task<CommandResult> UpdateAsync(IEnumerable<Team> teams, TokenAccess token);
    }
}