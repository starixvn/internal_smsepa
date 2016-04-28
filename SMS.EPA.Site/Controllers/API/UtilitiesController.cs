// -----------------------------------------------------------------------
// <copyright file="UtilitiesController.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.09">Created</change>
// </history>

namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using SMS.EPA.Criteria.Department;
    using SMS.EPA.Criteria.Employee;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Criteria.EPAHistory;
    using SMS.EPA.Criteria.KPI;
    using SMS.EPA.Criteria.KPIGroup;
    using SMS.EPA.Criteria.Period;
    using SMS.EPA.Criteria.TargetType;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.KPI;
    using SMS.EPA.Model.KPIGroup;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Model.TargetType;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class UtilitiesController : BaseController
    {
        private readonly ITargetTypeService _targetTypeService;
        private readonly IKPIService _kpiService;
        private readonly IKPIGroupService _kpiGroupService;
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IPeriodService _periodService;
        private readonly IEmployeeEPAService _employeeEPAService;
        private readonly IEPAHistoryService _epaHistoryService;

        public UtilitiesController()
            : this(null, null, null, null, null, null, null, null)
        {
        }

        public UtilitiesController(
            ITargetTypeService targetTypeService,
            IKPIService kpiService,
            IKPIGroupService kpiGroupService,
            IEmployeeService employeeService,
            IDepartmentService departmentService,
            IPeriodService periodService,
            IEmployeeEPAService employeeEPAService,
            IEPAHistoryService epaHistoryService)
        {
            _targetTypeService = targetTypeService ?? new TargetTypeService();
            _kpiService = kpiService ?? new KPIService();
            _kpiGroupService = kpiGroupService ?? new KPIGroupService();
            _employeeService = employeeService ?? new EmployeeService();
            _departmentService = departmentService ?? new DepartmentService();
            _periodService = periodService ?? new PeriodService();
            _employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            _epaHistoryService = epaHistoryService ?? new EPAHistoryService();
        }

        [NonAction]
        public IEnumerable<TargetTypeModel> GetTargetType(
            bool? isActive = null,
            string excludeTargetTypeIds = "")
        {
            var criteria = new GetTargetTypeCriteria
            {
                IsActive = isActive,
                ExcludeTargetTypeIds = excludeTargetTypeIds
            };

            return _targetTypeService.GetTargetType(criteria);
        }

        [NonAction]
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

            return _kpiService.GetKPI(criteria);
        }

        [NonAction]
        public IEnumerable<KPIGroupModel> GetKPIGroup(bool? isActive = null)
        {
            var criteria = new GetKPIGroupCriteria
            {
                IsActive = isActive
            };

            return _kpiGroupService.GetKPIGroup(criteria);
        }

        [NonAction]
        public IEnumerable<EmployeeModel> GetAllEmployee(
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
                ApproverId = getByApprover == true && CurrentUser != null ? (int?)CurrentUser.EmployeeInfo.EmployeeId : null,
                IncludeVerifier = includeVerifier
            };

            IEnumerable<EmployeeModel> employees = _employeeService.GetAllEmployee(criteria);
            return employees;
        }

        [NonAction]
        public IEnumerable<EmployeeModel> GetEmployeeByDepartmentId(int? departmentId = null)
        {
            var criteria = new GetEmployeeByDepartmentIdCriteria()
            {
                DepartmentId = departmentId
            };

            return _employeeService.GetEmployeeByDepartmentId(criteria);
        }

        [NonAction]
        public IEnumerable<EmployeeModel> GetEmployeeByGroupId(int groupId)
        {
            var criteria = new GetEmployeeByGroupIdCriteria()
            {
                GroupId = groupId
            };

            return _employeeService.GetEmployeeByGroupId(criteria);
        }

        [NonAction]
        public EmployeeModel GetEmployeeById(int employeeId)
        {
            var criteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = employeeId
            };

            return _employeeService.GetEmployeeById(criteria);
        }

        [NonAction]
        public IEnumerable<Group> GetAllGroup()
        {
            var criteria = new GetAllGroupCriteria();
            IEnumerable<Group> groups = _employeeService.GetAllGroup(criteria);
            return groups;
        }

        [NonAction]
        public DepartmentInfoModel GetDepartment(
            int? parentId = null,
            bool? isActive = null)
        {
            DepartmentModel treeList = null;
            var getDepartmentCriteria = new GetDepartmentCriteria()
            {
                ParentId = parentId,
                IsActive = isActive
            };

            IList<DepartmentModel> departments = _departmentService.GetDepartment(getDepartmentCriteria);
            if (departments != null && departments.Any())
            {
                treeList = new DepartmentModel();
                treeList.Childrens.Add(departments.Where(x => x.ParentId == null).SingleOrDefault());
            }

            var departmentInfo = new DepartmentInfoModel
            {
                TreeDepartment = treeList,
                Departments = departments
            };

            return departmentInfo;
        }

        [NonAction]
        public IEnumerable<PeriodModel> GetPeriod(
            int? targetTypeId = null,
            bool? isActive = true)
        {
            var criteria = new GetPeriodCriteria
            {
                TargetTypeId = targetTypeId,
                IsActive = isActive
            };

            return _periodService.GetPeriod(criteria);
        }

        [NonAction]
        public IEnumerable<StandardGradeModel> GetStandardGrade()
        {
            var criteria = new GetStandardGradeCriteria();

            return _employeeEPAService.GetStandardGrade(criteria);
        }

        [NonAction]
        public IList<PeriodModel> GetAllPeriodIncludeImport()
        {
            var criteria = new GetAllPeriodIncludeImportCriteria();
            IList<PeriodModel> periods = _epaHistoryService.GetAllPeriodIncludeImport(criteria);
            return periods;
        }

        [NonAction]
        public IEnumerable<EmployeeOtherViolationDetailModel> GetOtherViolationDetailByEPAId(int epaId)
        {
            var criteria = new GetOtherViolationDetailByEPAIdCriteria()
            {
                EPAId = epaId
            };

            return _employeeEPAService.GetOtherViolationDetailByEPAId(criteria);
        }
    }
}