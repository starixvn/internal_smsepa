namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Criteria.Department;
    using SMS.EPA.Criteria.EPAHistory;
    using SMS.EPA.Criteria.Period;
    using SMS.EPA.Criteria.TargetType;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.Period;
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

        public IEnumerable<TargetType> GetTargetType(GetTargetTypeCriteria criteria)
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

        public IEnumerable<Employee> GetAllEmployee(GetAllEmployeeCriteria criteria)
        {
            return employeeService.GetAllEmployee(criteria);
        }

        public IEnumerable<Employee> GetEmployeeByDepartmentId(int? departmentId)
        {
            return employeeService.GetEmployeeByDepartmentId(departmentId);
        }

        public IEnumerable<Employee> GetEmployeeByGroupId(int groupId)
        {
            return employeeService.GetEmployeeByGroupId(groupId);
        }

        public Employee GetEmployeeById(int employeeId)
        {
            return employeeService.GetEmployeeById(employeeId);
        }

        public IEnumerable<Group> GetAllGroup(GetAllGroupCriteria criteria)
        {
            var groups = GetData<Group>(criteria);
            return groups;
        }

        public DepartmentInfoModel GetDepartment(GetDepartmentCriteria criteria)
        {
            IList<Department> departments = departmentService.GetDepartment(criteria);
            Department treeList = GetRootDepartment(departments);
            var departmentInfo = new DepartmentInfoModel
            {
                TreeDepartment = treeList,
                Departments = departments
            };

            return departmentInfo;
        }

        public Department GetRootDepartment(IList<Department> departments)
        {
            Department treeList = null;
            if (departments != null && departments.Any())
            {
                treeList = new Department();
                treeList.Childrens.Add(departments.SingleOrDefault(x => x.ParentId == null));
            }

            return treeList;
        }

        public IEnumerable<PeriodModel> GetPeriod(GetPeriodCriteria criteria)
        {
            return periodService.GetPeriod(criteria);
        }

        public IEnumerable<StandardGradeModel> GetStandardGrade()
        {
            return employeeEPAService.GetStandardGrade();
        }

        public IList<PeriodModel> GetAllPeriodIncludeImport(GetAllPeriodIncludeImportCriteria criteria)
        {
            return epaHistoryService.GetAllPeriodIncludeImport(criteria);
        }

        public IEnumerable<EmployeeOtherViolationDetailModel> GetOtherViolationDetailByEPAId(GetOtherViolationDetailByEPAIdCriteria criteria)
        {
            return employeeEPAService.GetOtherViolationDetailByEPAId(criteria);
        }

        public IEnumerable<TargetTemplate> GetAllTargetTemplate(GetAllTargetTemplateCriteria criteria)
        {
            return targetTemplateService.GetAllTargetTemplate(criteria);
        }

        public IEnumerable<MenuGroupModel> GetMenusByEmployeeId(int employeeId)
        {
            var menus = menuService.GetMenusByEmployeeId(employeeId);

            return menus;
        }

        public IEnumerable<JobTitle> GetAllJobTitle()
        {
            var criteria = new GetAllJobTitleCriteria();
            IEnumerable<JobTitle> jobTitles = new List<JobTitle>();

            if (criteria.IsValid())
            {
                jobTitles = GetData<JobTitle>(criteria);
            }

            return jobTitles;
        }

        public IEnumerable<JobGradeModel> GetJobGrade(bool? isActive)
        {
            var criteria = new GetJobGradeCriteria
            {
                IsActive = isActive
            };
            IEnumerable<JobGradeModel> jobGrades = new List<JobGradeModel>();

            if (criteria.IsValid())
            {
                jobGrades = GetData<JobGradeModel>(criteria);
            }

            return jobGrades;
        }
    }
}