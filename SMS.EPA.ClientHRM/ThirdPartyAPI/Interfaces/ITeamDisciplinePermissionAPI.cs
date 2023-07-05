namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Model;
    using EPA.Model.API;
    using Model;

    public interface ITeamSupervisorAPI
    {
        Task<CommandResult> Update(IEnumerable<TeamDisciplinePermission> groupSupervisors, TokenAccess token);

        Task<DateTime> GetLastModifiedDateAsync(TokenAccess token);
    }
}