namespace SMS.EPA.Site.Controllers.API
{
    using Core.Attribute;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Model;
    using Criteria;
    using Model;
    using Services.Implement;
    using Services.Interfaces;
    using System.Collections.Generic;
    using System.Web.Http;

    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;

        public RoleController()
            : this(null)
        {
        }

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService ?? new RoleService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.RoleManagement, Policy = Policy.Read)]
        public IEnumerable<RoleFunctionModel> GetRoleFunctionByRoleId(int roleId)
        {
            IEnumerable<RoleFunctionModel> functions = _roleService.GetFunctionByRoleId(roleId);
            return functions;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RoleManagement, Policy = Policy.Host)]
        public CommandResult AssignFunctionToRole(AssignFunctionToRoleCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            CommandResult commandResult = _roleService.AssignFunctionToRole(criteria);
            return commandResult;
        }
    }
}