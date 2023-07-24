namespace SMS.EPA.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HRM.UserControl.Client.Core.Models.Role;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Criteria.EPAHistory;
    using SMS.EPA.Criteria.TargetType;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.JobDescription;

    public class CommonService : ICommonService
    {
        private readonly IKPIService kpiService;
        private readonly IBaseService baseService;
        private readonly IEmployeeService employeeService;
        private readonly IDepartmentService departmentService;
        private readonly IPeriodService periodService;

        public CommonService(
            IKPIService kpiService = null,
            IBaseService baseService = null,
            IEmployeeService employeeService = null,
            IDepartmentService departmentService = null,
            IPeriodService periodService = null)
        {
            this.kpiService = kpiService ?? new KPIService();
            this.baseService = baseService ?? new BaseService();
            this.employeeService = employeeService ?? new EmployeeService();
            this.departmentService = departmentService ?? new DepartmentService();
            this.periodService = periodService ?? new PeriodService();
        }

        public IEnumerable<TargetTypeModel> GetTargetType(GetTargetTypeCriteria criteria)
        {
            return baseService.GetData<TargetTypeModel>(criteria);
        }

        public IEnumerable<KPIModel> GetKPI(GetKPICriteria criteria)
        {
            return kpiService.GetKPI(criteria);
        }

        public IEnumerable<KPIGroupModel> GetKPIGroup(GetKPIGroupCriteria criteria)
        {
            return baseService.GetData<KPIGroupModel>(criteria);
        }

        public IEnumerable<Employee> GetEmployeeByDepartmentId(int? departmentId)
        {
            var criteria = new GetEmployeeByDepartmentIdCriteria
            {
                DepartmentId = departmentId
            };
            return baseService.GetData<Employee>(criteria).OrderBy(i => i.NickName);
        }

        public IEnumerable<Employee> GetEmployeeByGroupId(int groupId)
        {
            var criteria = new GetEmployeeByGroupIdCriteria
            {
                GroupId = groupId
            };
            return baseService.GetData<Employee>(criteria);
        }

        public Employee GetEmployeeById(int employeeId, IList<Role> roles)
        {
            return employeeService.GetEmployeeById(employeeId, roles);
        }

        public IEnumerable<Group> GetAllGroup(GetAllGroupCriteria criteria)
        {
            return baseService.GetData<Group>(criteria);
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

        private static Department GetRootDepartment(IList<Department> departments)
        {
            Department treeList = null;
            if (departments != null && departments.Any())
            {
                treeList = new Department();
                treeList.Childrens.AddRange(departments.Where(x => x.ParentId == null));
            }

            return treeList;
        }

        public IEnumerable<PeriodModel> GetPeriod(GetPeriodCriteria criteria)
        {
            return periodService.GetPeriod(criteria);
        }

        public IEnumerable<StandardGradeModel> GetStandardGrade()
        {
            return baseService.GetData<StandardGradeModel>(new GetStandardGradeCriteria());
        }

        public IEnumerable<PeriodModel> GetAllPeriodIncludeImport(GetAllPeriodIncludeImportCriteria criteria)
        {
            return baseService.GetData<PeriodModel>(criteria);
        }

        public IEnumerable<TargetTemplate> GetAllTargetTemplate(GetAllTargetTemplateCriteria criteria)
        {
            return baseService.GetData<TargetTemplate>(criteria);
        }

        public IEnumerable<JobTitle> GetAllJobTitle()
        {
            var criteria = new GetAllJobTitleCriteria();
            return baseService.GetData<JobTitle>(criteria);
        }

        public IEnumerable<ScoreRatingTypeModel> GetScoreRatingType(GetScoreRatingTypeCriteria criteria)
        {
            return baseService.GetData<ScoreRatingTypeModel>(criteria);
        }

        public IEnumerable<JobDescriptionGroupItem> GetJobDescriptionByEPAId(int epaId)
        {
            var criteria = new GetJobDescriptionByEPAIdCriteria { EPAId = epaId };
            var tuple = baseService.FetchMultiple<JobDescriptionGroupItem, JobDescriptionItem>(criteria);
            IEnumerable<JobDescriptionGroupItem> groupItems = tuple.Item1;
            IEnumerable<JobDescriptionItem> items = tuple.Item2;
            foreach (var groupitem in groupItems)
            {
                groupitem.Items = items.Where(x => x.GroupItemId == groupitem.GroupItemId).ToList();
            }

            return groupItems.OrderBy(x => x.DisplayOrder).ToList();
        }

        public string CalculateLengthOfService(DateTime? fromDate, DateTime? toDate, DateTime? militaryFromDate, DateTime? militaryToDate)
        {
            const byte HalfMonth = 15;
            const short DayOfYear = 365;
            const byte DayOfMonth = 30;
            const byte MonthOfYear = 12;
            int totalDays;
            int years;
            int months;
            int days;
            string lengOfService = string.Empty;
            if (fromDate.HasValue)
            {
                totalDays = ((TimeSpan)(toDate - fromDate)).Days;
                if (militaryToDate.HasValue && militaryFromDate.HasValue)
                {
                    totalDays -= ((TimeSpan)(militaryToDate - militaryFromDate)).Days;
                }
                years = totalDays / DayOfYear;
                months = (totalDays - (years * DayOfYear)) / DayOfMonth;
                days = totalDays - (years * DayOfYear) - (months * DayOfMonth);
                if (days >= HalfMonth)
                {
                    months++;
                }

                if (months >= MonthOfYear)
                {
                    years++;
                    months = 0;
                }

                lengOfService = string.Format("{0} Year(s) {1} Month(s)", years, months);
            }

            return lengOfService;
        }
    }
}