namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class RoleController : BaseController
    {
        private IRoleService roleService;

        public RoleController()
            : this(null)
        {
        }

        public RoleController(IRoleService roleService)
        {
            this.roleService = roleService ?? new RoleService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.RoleManagement, Policy = Policy.Read)]
        public IEnumerable<RoleFunctionModel> GetRoleFunctionByRoleId(int roleId)
        {
            var criteria = new GetFunctionByRoleIdCriteria
            {
                RoleId = roleId
            };

            IEnumerable<RoleFunctionModel> functions = roleService.GetFunctionByRoleId(criteria);
            return functions;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RoleManagement, Policy = Policy.Host)]
        public CommandResult AssignFunctionToRole(AssignFunctionToRoleCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = this.SessionService.EmployeeId;
            }

            CommandResult commandResult = roleService.AssignFunctionToRole(criteria);
            return commandResult;
        }
    }
}