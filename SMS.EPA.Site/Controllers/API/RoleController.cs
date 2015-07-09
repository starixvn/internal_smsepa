// ----------------------------------------------------------------------
// <copyright file="RoleController.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/9/2015 2:13:17 PM">Created.</change>
// </history>

namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.RoleManagement;
    using SMS.EPA.Model.RoleManagement;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    /// <summary>
    /// The role controller.
    /// </summary>
    public class RoleController : BaseController
    {
        private IRoleService _roleService;

        public RoleController()
            : this(null)
        {
        }

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService ?? new RoleService();
        }

        /// <summary>
        /// Get functions of role by role ID.
        /// </summary>
        /// <param name="roleId">The role ID.</param>
        /// <returns>The enumerator functions of role.</returns>
        public IEnumerable<RoleFunctionModel> GetRoleFunctionByRoleId(int roleId)
        {
            var criteria = new GetFunctionByRoleIdCriteria
            {
                RoleId = roleId
            };

            IEnumerable<RoleFunctionModel> functions = _roleService.GetFunctionByRoleId(criteria);
            return functions;
        }

        /// <summary>
        /// Assign function to role.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The command result.</returns>
        [HttpPut]
        public CommandResult AssignFunctionToRole(AssignFunctionToRoleCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            CommandResult commandResult = _roleService.AssignFunctionToRole(criteria);
            return commandResult;
        }
    }
}
