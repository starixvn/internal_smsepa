namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Model;
    using EPA.Model.API;
    using Model;

    public interface ITeamAPI
    {
        Task<DateTime> GetLastModifiedDate(TokenAccess token);

        Task<CommandResult> UpdateAsync(IEnumerable<Team> teams, TokenAccess token);
    }
}