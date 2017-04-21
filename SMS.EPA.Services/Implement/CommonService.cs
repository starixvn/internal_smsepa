namespace SMS.EPA.Services.Implement
{
    using Core.Services;
    using Criteria;
    using Criteria.EPAHistory;
    using Criteria.TargetType;
    using Interfaces;
    using Model;
    using Model.Enitites;
    using System.Collections.Generic;
    using System.Linq;

    public class CommonService : BaseService, ICommonService
    {
        private readonly ITargetTypeService _targetTypeService;
        private readonly IKPIService _kpiService;
        private readonly IKPIGroupService _kpiGroupService;
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IPeriodService _periodService;
        private readonly IEmployeeEPAService _employeeEPAService;
        private readonly IEPAHistoryService _epaHistoryService;
        private readonly ITargetTemplateService _targetTemplateService;
        private readonly IMenuService _menuService;

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
            _targetTypeService = targetTypeService ?? new TargetTypeService();
            _kpiService = kpiService ?? new KPIService();
            _kpiGroupService = kpiGroupService ?? new KPIGroupService();
            _employeeService = employeeService ?? new EmployeeService();
            _departmentService = departmentService ?? new DepartmentService();
            _periodService = periodService ?? new PeriodService();
            _employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            _epaHistoryService = epaHistoryService ?? new EPAHistoryService();
            _targetTemplateService = targetTemplateService ?? new TargetTemplateService();
            _menuService = menuService ?? new MenuService();
        }

        public IEnumerable<TargetTypeModel> GetTargetType(GetTargetTypeCriteria criteria)
        {
            return _targetTypeService.GetTargetType(criteria);
        }

        public IEnumerable<KPIModel> GetKPI(GetKPICriteria criteria)
        {
            return _kpiService.GetKPI(criteria);
        }

        public IEnumerable<KPIGroupModel> GetKPIGroup(GetKPIGroupCriteria criteria)
        {
            return _kpiGroupService.GetKPIGroup(criteria);
        }

        public IEnumerable<Employee> GetAllEmployee(GetAllEmployeeCriteria criteria)
        {
            return _employeeService.GetAllEmployee(criteria);
        }

        public IEnumerable<Employee> GetEmployeeByDepartmentId(int? departmentId)
        {
            return _employeeService.GetEmployeeByDepartmentId(departmentId);
        }

        public IEnumerable<Employee> GetEmployeeByGroupId(int groupId)
        {
            return _employeeService.GetEmployeeByGroupId(groupId);
        }

        public Employee GetEmployeeById(int employeeId)
        {
            return _employeeService.GetEmployeeById(employeeId);
        }

        public IEnumerable<Group> GetAllGroup(GetAllGroupCriteria criteria)
        {
            var groups = GetData<Group>(criteria);
            return groups;
        }

        public DepartmentInfoModel GetDepartment(GetDepartmentCriteria criteria)
        {
            IList<Department> departments = _departmentService.GetDepartment(criteria);
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
            return _periodService.GetPeriod(criteria);
        }

        public IEnumerable<StandardGradeModel> GetStandardGrade()
        {
            return _employeeEPAService.GetStandardGrade();
        }

        public IList<PeriodModel> GetAllPeriodIncludeImport(GetAllPeriodIncludeImportCriteria criteria)
        {
            return _epaHistoryService.GetAllPeriodIncludeImport(criteria);
        }

        public IEnumerable<EmployeeOtherViolationDetailModel> GetOtherViolationDetailByEPAId(GetOtherViolationDetailByEPAIdCriteria criteria)
        {
            return _employeeEPAService.GetOtherViolationDetailByEPAId(criteria);
        }

        public IEnumerable<TargetTemplate> GetAllTargetTemplate(GetAllTargetTemplateCriteria criteria)
        {
            return _targetTemplateService.GetAllTargetTemplate(criteria);
        }

        public IEnumerable<MenuGroupModel> GetMenusByEmployeeId(int employeeId)
        {
            var menus = _menuService.GetMenusByEmployeeId(employeeId);

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