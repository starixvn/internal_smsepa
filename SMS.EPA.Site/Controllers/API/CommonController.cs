namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using HRM.UserControl.Client.AspNetMvc.Session;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.Role;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Core.Utilities;
    using SMS.EPA.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Criteria.EPAHistory;
    using SMS.EPA.Criteria.TargetType;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Core.Enumerations;

    public class CommonController : BaseController
    {
        private readonly IRoleService roleService;
        private readonly IBaseService baseService;
        private readonly ICommonService commonService;

        public CommonController() : this(null, null, null)
        {
        }

        public CommonController(
            IRoleService roleService,
            IBaseService baseService,
            ICommonService commonService)
        {
            this.roleService = roleService ?? new RoleService();
            this.baseService = baseService ?? new BaseService();
            this.commonService = commonService ?? new CommonService();
        }

        [HttpGet]
        public IEnumerable<TargetTypeModel> GetTargetType(
            bool? isActive = true,
            string excludeTargetTypeIds = "")
        {
            var criteria = new GetTargetTypeCriteria
            {
                IsActive = isActive,
                ExcludeTargetTypeIds = excludeTargetTypeIds
            };

            return commonService.GetTargetType(criteria);
        }

        [HttpGet]
        public IEnumerable<KPIModel> GetKPI(
            int? kpiGroupId = null,
            bool? isActive = null,
            bool? isRequiredEvaluate = null)
        {
            var criteria = new GetKPICriteria
            {
                KPIGroupId = kpiGroupId,
                IsActive = isActive,
                IsRequiredEvaluate = isRequiredEvaluate
            };

            return commonService.GetKPI(criteria);
        }

        [HttpGet]
        public IEnumerable<KPIGroupModel> GetKPIGroup(bool? isActive = null)
        {
            var criteria = new GetKPIGroupCriteria
            {
                IsActive = isActive
            };

            return commonService.GetKPIGroup(criteria);
        }

        public IList<Role> GetSystemRoles(bool? isActive = true)
        {
            IList<Role> systemRoles = roleService.GetBySiteId(UserControlSession.SiteId, isActive);
            return systemRoles;
        }

        [HttpGet]
        public IList<Employee> SearchEmployee(
            string query = "",
            string teamIds = null,
            bool? isSpecialUser = null,
            bool? isDisabledUser = null,
            bool? getByApprover = null,
            bool? includeVerifier = null)
        {
            var criteria = new SearchEmployeeCriteria
            {
                Query = query,
                TeamIds = teamIds,
                IsSpecialUser = isSpecialUser,
                IsDisabledUser = isDisabledUser,
                ApproverId = getByApprover == true ? (int?)CurrentEmployeeId : null,
                IncludeVerifier = includeVerifier
            };

            return baseService.GetData<Employee>(criteria).ToList();
        }

        public IEnumerable<Employee> GetEmployeeByDepartmentId(int? departmentId = null)
        {
            return commonService.GetEmployeeByDepartmentId(departmentId);
        }

        [HttpGet]
        public IEnumerable<Employee> GetEmployeeByGroupId(int groupId)
        {
            return commonService.GetEmployeeByGroupId(groupId);
        }

        [HttpGet]
        public Employee GetEmployeeById(int employeeId)
        {
            IList<Role> roles = roleService.GetBySiteId(UserControlSession.SiteId);
            return commonService.GetEmployeeById(employeeId, roles);
        }

        [HttpGet]
        public IEnumerable<Group> GetAllGroup()
        {
            var criteria = new GetAllGroupCriteria();

            return commonService.GetAllGroup(criteria);
        }

        [HttpGet]
        public DepartmentInfoModel GetDepartment(int? parentId = null, bool? isActive = null)
        {
            var criteria = new GetDepartmentCriteria
            {
                ParentId = parentId,
                IsActive = isActive
            };

            return commonService.GetDepartment(criteria);
        }

        public List<object> GetAllStatus()
        {
            List<object> statuses = ((EpaStatus[])System.Enum.GetValues(typeof(EpaStatus)))
                .Select(x => new
                {
                    StatusId = (int)x,
                    Name = Enumerations.GetDescription(x)
                }).Cast<object>().ToList();

            return statuses;
        }

        [HttpGet]
        public IEnumerable<PeriodModel> GetPeriod(int? targetTypeId = null, bool? isActive = true)
        {
            var criteria = new GetPeriodCriteria
            {
                TargetTypeId = targetTypeId,
                IsActive = isActive
            };

            return commonService.GetPeriod(criteria);
        }

        [HttpGet]
        public IEnumerable<StandardGradeModel> GetStandardGrade()
        {
            return commonService.GetStandardGrade();
        }

        [HttpGet]
        public IEnumerable<PeriodModel> GetAllPeriodIncludeImport()
        {
            var criteria = new GetAllPeriodIncludeImportCriteria();

            return commonService.GetAllPeriodIncludeImport(criteria);
        }

        [HttpGet]
        public IEnumerable<TargetTemplate> GetAllTargetTemplate(int? targetTypeId = null, bool? isActive = null)
        {
            var criteria = new GetAllTargetTemplateCriteria
            {
                TargetTypeId = targetTypeId,
                IsActive = isActive
            };

            return commonService.GetAllTargetTemplate(criteria);
        }

        [HttpGet]
        public IEnumerable<MenuItem> GetMenusByEmployeeId()
        {
            IEnumerable<MenuItem> menus = UserControlSession.MenuItems;
            return GroupMenuItem(menus);
        }

        public IEnumerable<JobTitle> GetAllJobTitle()
        {
            return commonService.GetAllJobTitle();
        }

        public byte GetUserPermissionOnFlowManagement()
        {
            return GetPermissionOnFunction(EPAFunctionKey.FlowManagement);
        }

        public async Task<IEnumerable<JobLevel>> GetJobLevel(bool? isDeleted = null)
        {
            var criteria = new GetJobLevelCriteria { IsDeleted = isDeleted };
            return await baseService.GetDataAsync<JobLevel>(criteria);
        }

        public IList<MultiSelectItem> GetOrgChart()
        {
            var criteria = new GetOrgChartCriteria();
            var tuple = baseService.FetchMultiple<Department, Section, Team>(criteria);

            var departments = tuple.Item1.ToList();
            var sections = tuple.Item2.ToList();
            var teams = tuple.Item3.ToList();

            return GetOrgChart(departments, sections, teams);
        }

        private static IList<MenuItem> GroupMenuItem(IEnumerable<MenuItem> menuItems)
        {
            IList<MenuItem> parents = menuItems.Where(item => !item.ParentId.HasValue)
                .OrderBy(item => item.DisplayOrder).ToList();

            foreach (MenuItem parent in parents)
            {
                parent.Childs = menuItems.Where(x => x.ParentId.HasValue && x.ParentId.Value == parent.MenuItemId)
                    .OrderBy(menuItem => menuItem.DisplayOrder).ToList();
            }

            return parents;
        }
    }
}