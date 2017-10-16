namespace SMS.EPA.Services.Implement
{
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Interfaces;
    using Model;
    using System.Collections.Generic;
    using System.Linq;

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
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        private static IEnumerable<RoleFunctionModel> GroupFunction(IEnumerable<RoleFunctionModel> rawFunctions)
        {
            IEnumerable<RoleFunctionModel> functions = rawFunctions.Where(func => func.ParentFunctionId == null);

            foreach (RoleFunctionModel function in functions)
            {
                function.ChildFunctions = rawFunctions.Where(x => x.ParentFunctionId == function.FunctionId).ToList();
            }

            return functions;
        }
    }
}