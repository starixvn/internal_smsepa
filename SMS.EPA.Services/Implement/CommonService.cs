namespace SMS.EPA.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Services;
    using Criteria;
    using Criteria.EPAHistory;
    using Criteria.TargetType;
    using Interfaces;
    using Model;
    using Model.Enitites;
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
            var targetTypes = baseService.GetData<TargetTypeModel>(criteria);
            return targetTypes;
        }

        public IEnumerable<KPIModel> GetKPI(GetKPICriteria criteria)
        {
            return kpiService.GetKPI(criteria);
        }

        public IEnumerable<KPIGroupModel> GetKPIGroup(GetKPIGroupCriteria criteria)
        {
            return baseService.GetData<KPIGroupModel>(criteria);
        }

        public IEnumerable<Employee> GetAllEmployee(GetAllEmployeeCriteria criteria)
        {
            return employeeService.GetAllEmployee(criteria);
        }

        public IEnumerable<Employee> GetEmployeeByDepartmentId(int? departmentId)
        {
            var criteria = new GetEmployeeByDepartmentIdCriteria
            {
                DepartmentId = departmentId
            };
            IEnumerable<Employee> employees = baseService.GetData<Employee>(criteria).OrderBy(i => i.NickName);

            return employees;
        }

        public IEnumerable<Employee> GetEmployeeByGroupId(int groupId)
        {
            var criteria = new GetEmployeeByGroupIdCriteria
            {
                GroupId = groupId
            };
            IEnumerable<Employee> employees = baseService.GetData<Employee>(criteria);

            return employees;
        }

        public Employee GetEmployeeById(int employeeId)
        {
            return employeeService.GetEmployeeById(employeeId);
        }

        public IEnumerable<Group> GetAllGroup(GetAllGroupCriteria criteria)
        {
            var groups = baseService.GetData<Group>(criteria);
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

        private static Department GetRootDepartment(IList<Department> departments)
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
            return baseService.GetData<StandardGradeModel>(new GetStandardGradeCriteria());
        }

        public IEnumerable<PeriodModel> GetAllPeriodIncludeImport(GetAllPeriodIncludeImportCriteria criteria)
        {
            return baseService.GetData<PeriodModel>(criteria);
        }

        public IEnumerable<TargetTemplate> GetAllTargetTemplate(GetAllTargetTemplateCriteria criteria)
        {
            IEnumerable<TargetTemplate> targetTemplates = baseService.GetData<TargetTemplate>(criteria);
            return targetTemplates;
        }

        public IEnumerable<MenuGroupModel> GetMenusByEmployeeId(int employeeId)
        {
            var criteria = new GetMenusByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };

            var menuItems = baseService.GetData<MenuItemModel>(criteria);
            IList<MenuGroupModel> menus = GroupMenuItem(menuItems);

            return menus;
        }

        public IEnumerable<JobTitle> GetAllJobTitle()
        {
            var criteria = new GetAllJobTitleCriteria();
            IEnumerable<JobTitle> jobTitles = baseService.GetData<JobTitle>(criteria);

            return jobTitles;
        }

        public IEnumerable<ScoreRatingTypeModel> GetScoreRatingType(GetScoreRatingTypeCriteria criteria)
        {
            var scoreRatingTypes = baseService.GetData<ScoreRatingTypeModel>(criteria);
            return scoreRatingTypes;
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

        public string CalculateLengthOfService(DateTime fromDate, DateTime toDate)
        {
            if ((toDate.Year - fromDate.Year) > 0 || (fromDate.Month < toDate.Month))
            {
                const byte Eleven = 11;
                int years;
                int months;
                int daysInFromDateMonth = DateTime.DaysInMonth(fromDate.Year, fromDate.Month);
                int daysRemain = toDate.Day + (daysInFromDateMonth - fromDate.Day);

                if (toDate.Month > fromDate.Month)
                {
                    years = toDate.Year - fromDate.Year;
                    months = toDate.Month - (fromDate.Month + 1) + Math.Abs(daysRemain / daysInFromDateMonth);
                }
                else if (toDate.Month == fromDate.Month)
                {
                    if (toDate.Day >= fromDate.Day)
                    {
                        years = toDate.Year - fromDate.Year;
                        months = 0;
                    }
                    else
                    {
                        years = (toDate.Year - 1) - fromDate.Year;
                        months = Eleven;
                    }
                }
                else
                {
                    years = (toDate.Year - 1) - fromDate.Year;
                    months = toDate.Month + (Eleven - fromDate.Month) + Math.Abs(daysRemain / daysInFromDateMonth);
                }

                return string.Format("{0} Year(s) {1} Month(s)", years, months);
            }

            return string.Empty;
        }

        private IList<MenuGroupModel> GroupMenuItem(IEnumerable<MenuItemModel> menuItems)
        {
            var menuGroups = new List<MenuGroupModel>();
            IEnumerable<MenuItemModel> parentMenu = menuItems.Where(item => item.ParentId == null).OrderBy(item => item.DisplayOrder).ToList();

            foreach (MenuItemModel parent in parentMenu)
            {
                var menuGroupItem = new MenuGroupModel
                {
                    MenuId = parent.MenuId,
                    MenuName = parent.MenuName,
                    MenuLink = parent.MenuLink
                };

                menuGroupItem.MenuItems = menuItems.Where(menuItem => menuItem.ParentId == menuGroupItem.MenuId)
                    .OrderBy(menuItem => menuItem.DisplayOrder).ToList();

                menuGroups.Add(menuGroupItem);
            }

            return menuGroups;
        }
    }
}