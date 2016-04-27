using SMS.EPA.ClientHRM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.EPA.ClientHRM.Services
{
    public interface ITeamService
    {
        IEnumerable<Team> GetTeams(DateTime? fromDate);

        DateTime GetMaxModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<Team> teams, TokenAccess token);
    }
}
