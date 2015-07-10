// ----------------------------------------------------------------------
// <copyright file="RoleService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/9/2015 1:57:31 PM">Created.</change>
// </history>

namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Helper;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.RoleManagement;
    using SMS.EPA.Model.RoleManagement;
    using SMS.EPA.Services.Interfaces;

    public class RoleService : BaseService, IRoleService
    {
        public IEnumerable<RoleFunctionModel> GetFunctionByRoleId(GetFunctionByRoleIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<RoleFunctionModel> functions = new List<RoleFunctionModel>();

            if (criteria.IsValid())
            {
                IEnumerable<RoleFunctionModel> rawFunctions = GetData<RoleFunctionModel>(criteria);
                functions = GroupFunction(rawFunctions);
            }

            return functions;
        }

        public CommandResult AssignFunctionToRole(AssignFunctionToRoleCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        private IEnumerable<RoleFunctionModel> GroupFunction(IEnumerable<RoleFunctionModel> rawFunctions)
        {
            IEnumerable<RoleFunctionModel> functions = rawFunctions.Where<RoleFunctionModel>(func => func.ParentFunctionId == null);

            foreach (RoleFunctionModel function in functions)
            {
                function.ChildFunctions = rawFunctions.Where(x => x.ParentFunctionId == function.FunctionId).ToList();
            }

            return functions;
        }
    }
}
