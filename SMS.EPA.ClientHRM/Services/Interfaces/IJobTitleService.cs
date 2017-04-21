namespace SMS.EPA.ClientHrm.Services
{
    using Model;
    using System.Collections.Generic;

    public interface IJobTitleService
    {
        IEnumerable<JobTitle> GetJobTitles();

        CommandResult Update(IEnumerable<JobTitle> jobTitles, TokenAccess token);
    }
}