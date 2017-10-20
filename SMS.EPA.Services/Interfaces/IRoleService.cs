namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Model;

    public interface IRoleService
    {
        IEnumerable<RoleFunctionModel> GetFunctionByRoleId(int roleId);
    }
}