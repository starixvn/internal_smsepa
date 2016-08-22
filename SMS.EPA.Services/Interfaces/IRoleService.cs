namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;

    public interface IRoleService
    {
        IEnumerable<RoleFunctionModel> GetFunctionByRoleId(int roleId);

        CommandResult AssignFunctionToRole(AssignFunctionToRoleCriteria criteria);
    }
}