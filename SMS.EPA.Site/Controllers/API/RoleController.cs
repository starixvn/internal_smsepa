namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using Core.Attribute;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Model;
    using Services.Implement;
    using Services.Interfaces;

    public class RoleController : BaseController
    {
        private readonly IRoleService roleService;
        private readonly IBaseService baseService;

        public RoleController() : this(null, null)
        {
        }

        public RoleController(IRoleService roleService, IBaseService baseService)
        {
            this.roleService = roleService ?? new RoleService();
            this.baseService = baseService ?? new BaseService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.RoleManagement, Policy = Policy.Read)]
        public IEnumerable<RoleFunctionModel> GetRoleFunctionByRoleId(int roleId)
        {
            IEnumerable<RoleFunctionModel> functions = roleService.GetFunctionByRoleId(roleId);
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

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            return commandResult;
        }
    }
}