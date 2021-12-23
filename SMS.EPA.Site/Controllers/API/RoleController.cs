namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using HRM.UserControl.Client.AspNetMvc.Session;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;

    public class RoleController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IRoleService roleService;
        private readonly IFunctionService functionService;

        public RoleController() : this(null, null, null)
        {
        }

        public RoleController(
            IUserControlSession userControlSession,
            IRoleService roleService,
            IFunctionService functionService)
        {
            this.userControlSession = userControlSession ?? new UserControlSession();
            this.roleService = roleService ?? new RoleService();
            this.functionService = functionService ?? new FunctionService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.RoleManagement, Policy = Policy.Read)]
        public object GetRoleFunctionByRoleId(short roleId)
        {
            return new
            {
                Functions = functionService.GetHierarchyBySiteId(userControlSession.SiteId),
                FunctionPermissions = functionService.GetPermissionBySiteAndRoleId(userControlSession.SiteId, roleId)
            };
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RoleManagement, Policy = Policy.Host)]
        public CommandResult AssignFunctionToRole(short roleId, IList<FunctionPermission> permissions)
        {
            CommandResult commandResult = roleService.AssignPermission(userControlSession.SiteId, roleId, permissions);
            return commandResult;
        }
    }
}