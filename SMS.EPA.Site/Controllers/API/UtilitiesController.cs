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
    using System.Web.Http;
    using SMS.EPA.Criteria.Employee;
    using SMS.EPA.Criteria.KPI;
    using SMS.EPA.Criteria.KPIGroup;
    using SMS.EPA.Criteria.TargetType;
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

        public UtilitiesController()
            : this(null, null, null, null)
        {
        }

        public UtilitiesController(
            ITargetTypeService targetTypeService,
            IKPIService kpiService,
            IKPIGroupService kpiGroupService,
            IEmployeeService employeeService)
        {
            _targetTypeService = targetTypeService ?? new TargetTypeService();
            _kpiService = kpiService ?? new KPIService();
            _kpiGroupService = kpiGroupService ?? new KPIGroupService();
            _employeeService = employeeService ?? new EmployeeService();
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
        public KPIGroupModel GetKPIGroupById(int kpiGroupId)
        {
            var criteria = new GetKPIGroupByIdCriteria
            {
                KPIGroupId = kpiGroupId
            };

            return _kpiGroupService.GetKPIGroupById(criteria);
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

        public IEnumerable<Group> GetAllGroup()
        {
            var criteria = new GetAllGroupCriteria();
            IEnumerable<Group> groups = _employeeService.GetAllGroup(criteria);
            return groups;
        }
    }
}