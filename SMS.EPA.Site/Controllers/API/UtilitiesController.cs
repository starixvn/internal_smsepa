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
    using SMS.EPA.Criteria.KPI;
    using SMS.EPA.Criteria.KPIGroup;
    using SMS.EPA.Criteria.TargetType;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.KPI;
    using SMS.EPA.Model.KPIGroup;
    using SMS.EPA.Model.TargetType;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class UtilitiesController
    {
        private readonly ITargetTypeService _targetTypeService;
        private readonly IKPIService _kpiService;
        private readonly IKPIGroupService _kpiGroupService;
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;

        public UtilitiesController()
            : this(null, null, null, null, null)
        {
        }

        public UtilitiesController(
            ITargetTypeService targetTypeService,
            IKPIService kpiService,
            IKPIGroupService kpiGroupService,
            IEmployeeService employeeService,
            IDepartmentService departmentService)
        {
            _targetTypeService = targetTypeService ?? new TargetTypeService();
            _kpiService = kpiService ?? new KPIService();
            _kpiGroupService = kpiGroupService ?? new KPIGroupService();
            _employeeService = employeeService ?? new EmployeeService();
            _departmentService = departmentService ?? new DepartmentService();
        }

        [NonAction]
        public IEnumerable<TargetTypeModel> GetTargetType(bool? isActive = null)
        {
            var criteria = new GetTargetTypeCriteria
            {
                IsActive = isActive
            };

            return _targetTypeService.GetTargetType(criteria);
        }

        [NonAction]
        public IEnumerable<KPIModel> GetKPI(
            int? targetTypeId = null,
            int? kpiGroupId = null,
            bool? isActive = null,
            bool? isRequiredEvaluate = null)
        {
            var criteria = new GetKPICriteria
            {
                TargetTypeId = targetTypeId,
                KPIGroupId = kpiGroupId,
                IsActive = isActive,
                IsRequiredEvaluate = isRequiredEvaluate
            };

            return _kpiService.GetKPI(criteria);
        }

        [NonAction]
        public IEnumerable<KPIGroupModel> GetKPIGroup(
            int? targetTypeId = null,
            bool? isActive = null)
        {
            var criteria = new GetKPIGroupCriteria
            {
                TargetTypeId = targetTypeId,
                IsActive = isActive
            };

            return _kpiGroupService.GetKPIGroup(criteria);
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

            List<DepartmentModel> departments = _departmentService.GetDepartment(getDepartmentCriteria);
            if (departments != null)
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
    }
}