using SMS.EPA.ClientHRM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.EPA.ClientHRM.Service
{
    public interface IJobTitleService
    {
        IEnumerable<JobTitle> GetJobTitles();

        CommandResult Update(IEnumerable<JobTitle> jobtitles, TokenAccess token);
    }
}
