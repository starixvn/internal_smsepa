namespace SMS.EPA.ClientHrm.Services
{
    using Core.Model;
    using EPA.Model.API;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IJobLevelService
    {
        IEnumerable<JobLevel> GetJobLevel(DateTime? fromDate);

        Task<DateTime> GetLastModifiedDateAsync(TokenAccess token);

        Task<CommandResult> Update(IEnumerable<JobLevel> jobLevel, TokenAccess token);
    }
}