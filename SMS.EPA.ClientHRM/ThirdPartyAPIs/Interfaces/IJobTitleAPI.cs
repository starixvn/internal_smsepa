namespace SMS.EPA.ClientHRM.ThirdPartyAPIs
{
    using SMS.EPA.ClientHRM.Model;
    using System.Collections.Generic;

    public interface IJobTitleAPI
    {
        CommandResult Update(IEnumerable<JobTitle> departments, TokenAccess token);
    }
}
