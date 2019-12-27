namespace SMS.EPA.ClientHrm.ThirdPartyAPI
{
    using Core.Model;
    using EPA.Model.API;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDepartmentAPI
    {
        Task<DateTime> GetLastModifiedDate(TokenAccess token);

        Task<CommandResult> UpdateAsync(IEnumerable<Department> departments, TokenAccess token);
    }
}