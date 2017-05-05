namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using Core.Model;
    using Model;
    using System;
    using System.Collections.Generic;

    public interface ITeamSupervisorAPI
    {
        CommandResult Update(IEnumerable<GroupDisciplinePermission> groupSupervisors, TokenAccess token);

        DateTime GetLastModifiedDate(TokenAccess token);
    }
}