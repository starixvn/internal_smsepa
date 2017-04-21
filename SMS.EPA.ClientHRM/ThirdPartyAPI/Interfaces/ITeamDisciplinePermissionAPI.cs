namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.ClientHrm.Model;

    public interface ITeamSupervisorAPI
    {
        CommandResult Update(IEnumerable<GroupDisciplinePermission> groupSupervisors, TokenAccess token);

        DateTime GetLastModifiedDate(TokenAccess token);
    }
}