namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Utilities;
    using Criteria;
    using Criteria.EPAHistory;
    using Criteria.TargetType;
    using Model;
    using Model.Enitites;
    using Services.Implement;
    using Services.Interfaces;

    public class CommonController : BaseController
    {
        private readonly ICommonService commonService;

        public CommonController() : this(null)
        {
        }

        public CommonController(ICommonService commonService)
        {
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

        public List<object> GetSystemRoles(bool exceptNone)
        {
            var roles = new List<object>();

            foreach (SystemRole roleEnum in (SystemRole[])System.Enum.GetValues(typeof(SystemRole)))
            {
                var role = new
                {
                    RoleId = (int)roleEnum,
                    RoleCode = Enumerations.GetDescription(roleEnum)
                };

                if (exceptNone && roleEnum != 0)
                {
                    roles.Add(role);
                }
                else
                {
                    if (!exceptNone)
                    {
                        roles.Add(role);
                    }
                }
            }

            return roles;
        }

        public IEnumerable<Employee> GetAllEmployee(
            string query = "",
            int? groupId = null,
            bool? isSpecialUser = null,
            bool? isDisabledUser = null,
            bool? getByApprover = null,
            bool? includeVerifier = null)
        {
            var criteria = new GetAllEmployeeCriteria
            {
                Query = query,
                GroupId = groupId,
                IsSpecialUser = isSpecialUser,
                IsDisabledUser = isDisabledUser,
                ApproverId = getByApprover == true && SessionService != null ? (int?)SessionService.EmployeeId : null,
                IncludeVerifier = includeVerifier
            };

            return commonService.GetAllEmployee(criteria);
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
            return commonService.GetEmployeeById(employeeId);
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
            List<object> statusList = ((EPAStatus[])System.Enum.GetValues(typeof(EPAStatus)))
                .Select(x => new
                {
                    StatusId = (int)x,
                    Name = Enumerations.GetDescription(x)
                }).Cast<object>().ToList();

            return statusList;
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
        public IList<PeriodModel> GetAllPeriodIncludeImport()
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
        public IEnumerable<MenuGroupModel> GetMenusByEmployeeId()
        {
            return commonService.GetMenusByEmployeeId(SessionService.EmployeeId);
        }

        public IEnumerable<JobTitle> GetAllJobTitle()
        {
            return commonService.GetAllJobTitle();
        }

        public int GetUserPermissionOnFlowManagement()
        {
            int permission = SessionService.GetPermission(EPAFunctionKey.FlowManagement);
            return permission;
        }

        [HttpGet]
        public IEnumerable<JobGradeModel> GetJobGrade(bool? isActive)
        {
            return commonService.GetJobGrade(isActive);
        }
    }
}