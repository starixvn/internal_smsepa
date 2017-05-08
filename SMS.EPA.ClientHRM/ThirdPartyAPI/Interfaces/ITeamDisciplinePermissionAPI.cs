namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using Core.Model;
    using EPA.Model.API;
    using Model;
    using System;
    using System.Collections.Generic;

    public interface ITeamSupervisorAPI
    {
        CommandResult Update(IEnumerable<TeamDisciplinePermission> groupSupervisors, TokenAccess token);

        DateTime GetLastModifiedDate(TokenAccess token);
    }
}