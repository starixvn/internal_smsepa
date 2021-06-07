namespace SMS.EPA.WindowsService.ThirdPartyAPI
{
    using Core.Model;
    using EPA.Model.API;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITeamAPI
    {
        Task<DateTime> GetLastModifiedDate(TokenAccess token);

        Task<CommandResult> UpdateAsync(IEnumerable<Team> teams, TokenAccess token);
    }
}