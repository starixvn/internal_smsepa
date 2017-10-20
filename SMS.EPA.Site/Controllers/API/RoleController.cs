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
        private readonly IRoleService roleService;
        private readonly ICommonService commonService;

        public RoleController() : this(null, null)
        {
        }

        public RoleController(IRoleService roleService, ICommonService commonService)
        {
            this.roleService = roleService ?? new RoleService();
            this.commonService = commonService ?? new CommonService();
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

            CommandResult commandResult = commonService.ExecuteNonQuery(criteria);
            return commandResult;
        }
    }
}