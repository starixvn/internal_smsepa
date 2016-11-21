namespace SMS.EPA.ClientHRM.Services
{
    using SMS.EPA.ClientHRM.Model;
    using System.Collections.Generic;

    public interface IJobTitleService
    {
        IEnumerable<JobTitle> GetJobTitles();

        CommandResult Update(IEnumerable<JobTitle> jobTitles, TokenAccess token);
    }
}
