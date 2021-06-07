namespace SMS.EPA.WindowsService.ThirdPartyAPI
{
    using Core.Model;
    using EPA.Model.API;
    using Model;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IJobTitleAPI
    {
        Task<CommandResult> UpdateAsync(IEnumerable<JobTitle> jobTitles, TokenAccess token);
    }
}