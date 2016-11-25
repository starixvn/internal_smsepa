namespace SMS.EPA.ClientHrm.ThirdPartyAPIs
{
    using System.Collections.Generic;
    using SMS.EPA.ClientHrm.Model;

    public interface IJobTitleAPI
    {
        CommandResult Update(IEnumerable<JobTitle> departments, TokenAccess token);
    }
}