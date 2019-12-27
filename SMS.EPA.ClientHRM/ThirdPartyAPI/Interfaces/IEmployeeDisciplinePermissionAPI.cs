namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using Core.Model;
    using EPA.Model.API;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IEmployeeSupervisorAPI
    {
        Task<CommandResult> UpdateAsync(IEnumerable<EmployeeDisciplinePermission> employeeSupervisors, TokenAccess token);

        Task<DateTime> GetLastModifiedDateAsync(TokenAccess token);
    }
}