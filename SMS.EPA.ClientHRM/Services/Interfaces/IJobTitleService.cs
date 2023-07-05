namespace SMS.EPA.ClientHrm.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Model;
    using EPA.Model.API;
    using Model;

    public interface IJobTitleService
    {
        IEnumerable<JobTitle> GetJobTitles();

        Task<CommandResult> UpdateAsync(IEnumerable<JobTitle> jobTitles, TokenAccess token);
    }
}