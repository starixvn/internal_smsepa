namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Core.Model;
    using Criteria;
    using Model;

    public interface IRoleService
    {
        IEnumerable<RoleFunctionModel> GetFunctionByRoleId(int roleId);

        CommandResult AssignFunctionToRole(AssignFunctionToRoleCriteria criteria);
    }
}