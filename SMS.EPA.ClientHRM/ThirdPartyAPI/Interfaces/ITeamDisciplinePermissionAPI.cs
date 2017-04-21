namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using System;
    using System.Collections.Generic;
    using Model;

    public interface ITeamSupervisorAPI
    {
        CommandResult Update(IEnumerable<GroupDisciplinePermission> groupSupervisors, TokenAccess token);

        DateTime GetLastModifiedDate(TokenAccess token);
    }
}