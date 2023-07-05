namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Model;
    using EPA.Model.API;
    using Model;

    public interface IEmployeeSupervisorAPI
    {
        Task<CommandResult> UpdateAsync(IEnumerable<EmployeeDisciplinePermission> employeeSupervisors, TokenAccess token);

        Task<DateTime> GetLastModifiedDateAsync(TokenAccess token);
    }
}