namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Utilities;
    using SMS.EPA.Criteria;
    using SMS.EPA.Criteria.Department;
    using SMS.EPA.Criteria.Employee;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Criteria.EPAHistory;
    using SMS.EPA.Criteria.KPIGroup;
    using SMS.EPA.Criteria.Period;
    using SMS.EPA.Criteria.TargetTemplate;
    using SMS.EPA.Criteria.TargetType;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Model.TargetTemplate;
    using SMS.EPA.Model.TargetType;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class CommonController : BaseController
    {
        private readonly ICommonService commonService;
        private readonly IExportPdfService exportPdfService;

        public CommonController()
            : this(null, null)
        {
        }

        public CommonController(ICommonService commonService, IExportPdfService exportPdfService)
        {
            this.commonService = commonService ?? new CommonService();
            this.exportPdfService = exportPdfService ?? new ExportPdfService();
        }

        [HttpGet]
        public IEnumerable<TargetTypeModel> GetTargetType(
            bool? isActive = true,
            string excludeTargetTypeIds = "")
        {
            var criteria = new GetTargetTypeCriteria()
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
            var criteria = new GetKPICriteria()
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
            var criteria = new GetKPIGroupCriteria()
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
                else if (!exceptNone)
                {
                    roles.Add(role);
                }
            }

            return roles;
        }

        public IEnumerable<EmployeeModel> GetAllEmployee(
            string query = "",
            int? groupId = null,
            bool? isSpecialUser = null,
            bool? isDisabledUser = null,
            bool? getByApprover = null,
            bool? includeVerifier = null)
        {
            var criteria = new GetAllEmployeeCriteria()
            {
                Query = query,
                GroupId = groupId,
                IsSpecialUser = isSpecialUser,
                IsDisabledUser = isDisabledUser,
                ApproverId = getByApprover == true && CurrentUser != null ? (int?)CurrentUser.EmployeeId : null,
                IncludeVerifier = includeVerifier
            };

            return commonService.GetAllEmployee(criteria);
        }

        public IEnumerable<EmployeeModel> GetEmployeeByDepartmentId(int? departmentId = null)
        {
            var criteria = new GetEmployeeByDepartmentIdCriteria()
            {
                DepartmentId = departmentId
            };

            return commonService.GetEmployeeByDepartmentId(criteria);
        }

        [HttpGet]
        public IEnumerable<EmployeeModel> GetEmployeeByGroupId(int groupId)
        {
            var criteria = new GetEmployeeByGroupIdCriteria()
            {
                GroupId = groupId
            };

            return commonService.GetEmployeeByGroupId(criteria);
        }

        [HttpGet]
        public EmployeeModel GetEmployeeById(int employeeId)
        {
            var criteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = employeeId
            };

            return commonService.GetEmployeeById(criteria);
        }

        [HttpGet]
        public IEnumerable<Group> GetAllGroup()
        {
            var criteria = new GetAllGroupCriteria();

            return commonService.GetAllGroup(criteria);
        }

        [HttpGet]
        public DepartmentInfoModel GetDepartment(
            int? parentId = null,
            bool? isActive = null)
        {
            var criteria = new GetDepartmentCriteria()
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
        public IEnumerable<PeriodModel> GetPeriod(
            int? targetTypeId = null,
            bool? isActive = true)
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
            var criteria = new GetStandardGradeCriteria();

            return commonService.GetStandardGrade(criteria);
        }

        [HttpGet]
        public IList<PeriodModel> GetAllPeriodIncludeImport()
        {
            var criteria = new GetAllPeriodIncludeImportCriteria();

            return commonService.GetAllPeriodIncludeImport(criteria);
        }

        [HttpGet]
        public IEnumerable<EmployeeOtherViolationDetailModel> GetOtherViolationDetailByEPAId(int epaId)
        {
            var criteria = new GetOtherViolationDetailByEPAIdCriteria()
            {
                EPAId = epaId
            };

            return commonService.GetOtherViolationDetailByEPAId(criteria);
        }

        [HttpGet]
        public IEnumerable<TargetTemplateModel> GetAllTargetTemplate(int? targetTypeId = null, bool? isActive = null)
        {
            var criteria = new GetAllTargetTemplateCriteria
            {
                TargetTypeId = targetTypeId,
                IsActive = isActive
            };

            return commonService.GetAllTargetTemplate(criteria);
        }

        [HttpGet]
        public bool DisplayOnNormalWidthPage(int epaId)
        {
            return exportPdfService.DisplayOnNormalWidthPage(epaId);
        }
    }
}