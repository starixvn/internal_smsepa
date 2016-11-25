namespace SMS.EPA.ClientHrm.Services
{
    using System.Collections.Generic;
    using SMS.EPA.ClientHrm.Model;

    public interface IJobTitleService
    {
        IEnumerable<JobTitle> GetJobTitles();

        CommandResult Update(IEnumerable<JobTitle> jobTitles, TokenAccess token);
    }
}