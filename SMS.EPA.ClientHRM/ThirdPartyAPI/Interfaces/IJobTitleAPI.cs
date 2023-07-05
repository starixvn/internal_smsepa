namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Model;
    using EPA.Model.API;
    using Model;

    public interface IJobTitleAPI
    {
        Task<CommandResult> UpdateAsync(IEnumerable<JobTitle> jobTitles, TokenAccess token);
    }
}