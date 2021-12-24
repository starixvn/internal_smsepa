namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using HRM.UserControl.Client.AspNetMvc.Session;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Model;

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
            IList<Function> functions = functionService.GetBySiteId(userControlSession.SiteId);
            var permissions = functionService.GetPermissionBySiteAndRoleId(userControlSession.SiteId, roleId);
            var roleFunctions = new List<RoleFunctionModel>();
            foreach(var function in functions)
            {
                var permission = permissions.FirstOrDefault(x => x.FunctionId == function.FunctionId);
                roleFunctions.Add(new RoleFunctionModel
                {
                    FunctionId = function.FunctionId,
                    FunctionName = function.FunctionName,
                    FunctionKey = function.FunctionKey,
                    ParentId = function.ParentId,
                    Policy = function.Policy,
                    Level = function.Level,
                    DisplayOrder= function.DisplayOrder,
                    Childs = new List<RoleFunctionModel>(),
                    IsRead = permission != null ? permission.IsRead : false,
                    IsHost = permission != null ? permission.IsHost : false
                });
            }

            return GetHierarchyFunction(roleFunctions);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.RoleManagement, Policy = Policy.Host)]
        public CommandResult AssignFunctionToRole(AssignFunctionToRole model)
        {
            CommandResult commandResult = roleService.AssignPermission(userControlSession.SiteId, model.RoleId, model.Permissions);
            return commandResult;
        }

        private IList<RoleFunctionModel> GetHierarchyFunction(IList<RoleFunctionModel> functions)
        {
            IList<RoleFunctionModel> functionTrees = functions.Where(func => !func.ParentId.HasValue)
                .OrderBy(x => x.DisplayOrder).ToList();
            foreach (RoleFunctionModel function in functionTrees)
            {
                function.Level = 0;
                GetChildFunctions(function, functions);
            }

            return functionTrees;
        }

        private void GetChildFunctions(RoleFunctionModel function, IList<RoleFunctionModel> functions)
        {
            IList<RoleFunctionModel> functionItems = functions.Where(x => x.ParentId == function.FunctionId)
                .OrderBy(x => x.DisplayOrder).ToList();
            if (functionItems.Any())
            {
                function.Childs = function.Childs ?? new List<RoleFunctionModel>();
                foreach (RoleFunctionModel item in functionItems)
                {
                    item.Level = (byte)(function.Level + 1);
                    function.Childs.Add(item);
                    GetChildFunctions(item, functions);
                }
            }
        }
    }
}