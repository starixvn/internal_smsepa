namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Interfaces;

    public class RoleService : BaseService, IRoleService
    {
        public IEnumerable<RoleFunctionModel> GetFunctionByRoleId(int roleId)
        {
            var criteria = new GetFunctionByRoleIdCriteria
            {
                RoleId = roleId
            };
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
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
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