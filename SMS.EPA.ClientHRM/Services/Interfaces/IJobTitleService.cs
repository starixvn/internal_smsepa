namespace SMS.EPA.ClientHrm.Services
{
    using Core.Model;
    using EPA.Model.API;
    using Model;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IJobTitleService
    {
        IEnumerable<JobTitle> GetJobTitles();

        Task<CommandResult> UpdateAsync(IEnumerable<JobTitle> jobTitles, TokenAccess token);
    }
}