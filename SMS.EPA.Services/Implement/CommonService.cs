namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Criteria.Department;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Criteria.EPAHistory;
    using SMS.EPA.Criteria.Period;
    using SMS.EPA.Criteria.TargetType;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Model.TargetType;
    using SMS.EPA.Services.Interfaces;

    public class CommonService : BaseService, ICommonService
    {
        private readonly ITargetTypeService targetTypeService;
        private readonly IKPIService kpiService;
        private readonly IKPIGroupService kpiGroupService;
        private readonly IEmployeeService employeeService;
        private readonly IDepartmentService departmentService;
        private readonly IPeriodService periodService;
        private readonly IEmployeeEPAService employeeEPAService;
        private readonly IEPAHistoryService epaHistoryService;
        private readonly ITargetTemplateService targetTemplateService;
        private readonly IMenuService menuService;

        public CommonService()
            : this(null, null, null, null, null, null, null, null, null, null)
        {
        }

        public CommonService(
            ITargetTypeService targetTypeService,
            IKPIService kpiService,
            IKPIGroupService kpiGroupService,
            IEmployeeService employeeService,
            IDepartmentService departmentService,
            IPeriodService periodService,
            IEmployeeEPAService employeeEPAService,
            IEPAHistoryService epaHistoryService,
            ITargetTemplateService targetTemplateService,
            IMenuService menuService)
        {
            this.targetTypeService = targetTypeService ?? new TargetTypeService();
            this.kpiService = kpiService ?? new KPIService();
            this.kpiGroupService = kpiGroupService ?? new KPIGroupService();
            this.employeeService = employeeService ?? new EmployeeService();
            this.departmentService = departmentService ?? new DepartmentService();
            this.periodService = periodService ?? new PeriodService();
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            this.epaHistoryService = epaHistoryService ?? new EPAHistoryService();
            this.targetTemplateService = targetTemplateService ?? new TargetTemplateService();
            this.menuService = menuService ?? new MenuService();
        }

        public IEnumerable<TargetTypeModel> GetTargetType(GetTargetTypeCriteria criteria)
        {
            return targetTypeService.GetTargetType(criteria);
        }

        public IEnumerable<KPIModel> GetKPI(GetKPICriteria criteria)
        {
            return kpiService.GetKPI(criteria);
        }

        public IEnumerable<KPIGroupModel> GetKPIGroup(GetKPIGroupCriteria criteria)
        {
            return kpiGroupService.GetKPIGroup(criteria);
        }

        public IEnumerable<EmployeeModel> GetAllEmployee(GetAllEmployeeCriteria criteria)
        {
            return employeeService.GetAllEmployee(criteria);
        }

        public IEnumerable<EmployeeModel> GetEmployeeByDepartmentId(GetEmployeeByDepartmentIdCriteria criteria)
        {
            return employeeService.GetEmployeeByDepartmentId(criteria);
        }

        public IEnumerable<EmployeeModel> GetEmployeeByGroupId(GetEmployeeByGroupIdCriteria criteria)
        {
            return employeeService.GetEmployeeByGroupId(criteria);
        }

        public EmployeeModel GetEmployeeById(GetEmployeeByIdCriteria criteria)
        {
            return employeeService.GetEmployeeById(criteria);
        }

        public IEnumerable<Group> GetAllGroup(GetAllGroupCriteria criteria)
        {
            IEnumerable<Group> groups = new List<Group>();

            if (criteria != null && criteria.IsValid())
            {
                groups = GetData<Group>(criteria);
            }

            return groups;
        }

        public DepartmentInfoModel GetDepartment(GetDepartmentCriteria criteria)
        {
            DepartmentModel treeList = null;

            IList<DepartmentModel> departments = departmentService.GetDepartment(criteria);
            if (departments != null && departments.Any())
            {
                treeList = new DepartmentModel();
                treeList.Childrens.Add(departments.SingleOrDefault(x => x.ParentId == null));
            }

            var departmentInfo = new DepartmentInfoModel
            {
                TreeDepartment = treeList,
                Departments = departments
            };

            return departmentInfo;
        }

        public IEnumerable<PeriodModel> GetPeriod(GetPeriodCriteria criteria)
        {
            return periodService.GetPeriod(criteria);
        }

        public IEnumerable<StandardGradeModel> GetStandardGrade(GetStandardGradeCriteria criteria)
        {
            return employeeEPAService.GetStandardGrade(criteria);
        }

        public IList<PeriodModel> GetAllPeriodIncludeImport(GetAllPeriodIncludeImportCriteria criteria)
        {
            return epaHistoryService.GetAllPeriodIncludeImport(criteria);
        }

        public IEnumerable<EmployeeOtherViolationDetailModel> GetOtherViolationDetailByEPAId(GetOtherViolationDetailByEPAIdCriteria criteria)
        {
            return employeeEPAService.GetOtherViolationDetailByEPAId(criteria);
        }

        public IEnumerable<TargetTemplateModel> GetAllTargetTemplate(GetAllTargetTemplateCriteria criteria)
        {
            return targetTemplateService.GetAllTargetTemplate(criteria);
        }

        public IEnumerable<MenuGroupModel> GetMenusByEmployeeId(int employeeId)
        {
            var menus = menuService.GetMenusByEmployeeId(employeeId);

            return menus;
        }

        public IEnumerable<JobTitle> GetAllJobTitle(GetAllJobTitleCriteria criteria)
        {
            IEnumerable<JobTitle> jobTitles = new List<JobTitle>();

            if (criteria != null && criteria.IsValid())
            {
                jobTitles = GetData<JobTitle>(criteria);
            }

            return jobTitles;
        }
    }
}