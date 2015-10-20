// -----------------------------------------------------------------------
// <copyright file="CommonController.cs" company="Nexcel Solutions Viet Nam">
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
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Utilities;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.KPI;
    using SMS.EPA.Model.KPIGroup;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Model.TargetType;

    public class CommonController : BaseController
    {
        private readonly UtilitiesController _utilitiesController;

        public CommonController()
            : this(null)
        {
        }

        public CommonController(UtilitiesController utilitiesController)
        {
            _utilitiesController = utilitiesController ?? new UtilitiesController();
        }

        [HttpGet]
        public IEnumerable<TargetTypeModel> GetTargetType(bool? isActive = true)
        {
            return _utilitiesController.GetTargetType(isActive);
        }

        [HttpGet]
        public IEnumerable<KPIModel> GetKPI(
            int? kpiGroupId = null,
            bool? isActive = null,
            bool? isRequiredEvaluate = null)
        {
            return _utilitiesController.GetKPI(kpiGroupId, isActive, isRequiredEvaluate);
        }

        [HttpGet]
        public IEnumerable<KPIGroupModel> GetKPIGroup(bool? isActive = null)
        {
            return _utilitiesController.GetKPIGroup(isActive);
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
            return _utilitiesController.GetAllEmployee(query, groupId, isSpecialUser, isDisabledUser, getByApprover, includeVerifier);
        }

        public IEnumerable<EmployeeModel> GetEmployeeByDepartmentId(int? departmentId = null)
        {
            return _utilitiesController.GetEmployeeByDepartmentId(departmentId);
        }

        [HttpGet]
        public IEnumerable<EmployeeModel> GetEmployeeByGroupId(int groupId)
        {
            return _utilitiesController.GetEmployeeByGroupId(groupId);
        }

        [HttpGet]
        public EmployeeModel GetEmployeeById(int employeeId)
        {
            EmployeeModel employee = _utilitiesController.GetEmployeeById(employeeId);

            return employee;
        }

        [HttpGet]
        public IEnumerable<Group> GetAllGroup()
        {
            IEnumerable<Group> groups = _utilitiesController.GetAllGroup();

            return groups;
        }

        [HttpGet]
        public DepartmentInfoModel GetDepartment(
            int? parentId = null,
            bool? isActive = null)
        {
            return _utilitiesController.GetDepartment(parentId, isActive);
        }

        public List<object> GetAllStatus()
        {
            var statusList = new List<object>();

            foreach (Status statusEnum in (Status[])System.Enum.GetValues(typeof(Status)))
            {
                var status = new
                {
                    StatusId = (int)statusEnum,
                    Name = Enumerations.GetDescription(statusEnum)
                };

                statusList.Add(status);
            }

            return statusList;
        }

        [HttpGet]
        public IEnumerable<PeriodModel> GetPeriod(
            int? targetTypeId = null,
            bool? isActive = true)
        {
            return _utilitiesController.GetPeriod(targetTypeId, isActive);
        }

        [HttpGet]
        public IEnumerable<StandardGradeModel> GetStandardGrade()
        {
            return _utilitiesController.GetStandardGrade();
        }

        [HttpGet]
        public IList<PeriodModel> GetAllPeriodIncludeImport()
        {
            return _utilitiesController.GetAllPeriodIncludeImport();
        }
    }
}