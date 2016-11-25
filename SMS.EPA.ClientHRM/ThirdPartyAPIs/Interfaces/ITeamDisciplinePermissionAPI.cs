namespace SMS.EPA.ClientHrm.ThirdPartyAPIs
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.ClientHrm.Model;

    public interface ITeamSupervisorAPI
    {
        CommandResult Update(IEnumerable<GroupDisciplinePermission> groups, TokenAccess token);

        DateTime GetLastModifiedDate(TokenAccess token);
    }
}