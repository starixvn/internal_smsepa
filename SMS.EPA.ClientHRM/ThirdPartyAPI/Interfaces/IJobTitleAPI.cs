namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using System.Collections.Generic;
    using SMS.EPA.ClientHrm.Model;

    public interface IJobTitleAPI
    {
        CommandResult Update(IEnumerable<JobTitle> jobTitles, TokenAccess token);
    }
}