// ----------------------------------------------------------------------
// <copyright file="IRoleService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/9/2015 1:55:34 PM">Created.</change>
// </history>

namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.RoleManagement;
    using SMS.EPA.Model.RoleManagement;

    /// <summary>
    /// The interface of role service.
    /// </summary>
    public interface IRoleService
    {
        IEnumerable<RoleFunctionModel> GetFunctionByRoleId(GetFunctionByRoleIdCriteria criteria);

        CommandResult AssignFunctionToRole(AssignFunctionToRoleCriteria criteria);
    }
}
