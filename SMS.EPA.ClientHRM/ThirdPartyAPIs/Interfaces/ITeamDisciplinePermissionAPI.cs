namespace SMS.EPA.ClientHRM.ThirdPartyAPIs
{
    using SMS.EPA.ClientHRM.Model;
    using System;
    using System.Collections.Generic;

    public interface ITeamSupervisorAPI
    {
        CommandResult Update(IEnumerable<GroupSupervisor> groups, TokenAccess token);

        DateTime GetMaxModifiedDate(TokenAccess token);
    }
}
