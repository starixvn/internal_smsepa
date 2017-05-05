namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using Core.Model;
    using Model;
    using System.Collections.Generic;

    public interface IJobTitleAPI
    {
        CommandResult Update(IEnumerable<JobTitle> jobTitles, TokenAccess token);
    }
}