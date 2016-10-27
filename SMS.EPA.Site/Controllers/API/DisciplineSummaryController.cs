namespace SMS.EPA.Site.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class DisciplineSummaryController : BaseController
    {
        private readonly IDisciplineService disciplineService;

        public DisciplineSummaryController()
            : this(null)
        {
        }

        public DisciplineSummaryController(IDisciplineService disciplineService)
        {
            this.disciplineService = disciplineService ?? new DisciplineService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.DisciplineSummary, Policy = Policy.Read)]
        public IEnumerable<DisciplineSummaryModel> GetDisciplineSummary(
            DateTime startDate,
            DateTime endDate,
            int? departmentId = null,
            bool? isLocal = null,
            int? employeeId = null,
            int? reasonId = null,
            bool isShowAll = false)
        {
            var criteria = new GetDisciplineSummaryCriteria
            {
                StartDate = startDate,
                EndDate = endDate,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                EmployeeId = employeeId,
                ReasonId = reasonId,
                ApproverId = this.SessionService.EmployeeId,
                IsShowAll = isShowAll
            };
            IEnumerable<DisciplineSummaryModel> employeeDisciplines = disciplineService.GetDisciplineSummary(criteria);

            return employeeDisciplines;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.DisciplineSummary, Policy = Policy.Read)]
        public IEnumerable<DisciplineDetailModel> GetDisciplineDetail(
            DateTime startDate,
            DateTime endDate,
            int? employeeId = null,
            int? departmentId = null,
            bool? isLocal = null,
            int? reasonId = null,
            bool? hasIgnored = null)
        {
            var criteria = new GetDisciplineDetailCriteria
            {
                StartDate = startDate,
                EndDate = endDate,
                EmployeeId = employeeId,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                ReasonId = reasonId,
                ApproverId = this.SessionService.EmployeeId,
                HasIgnored = hasIgnored
            };
            IEnumerable<DisciplineDetailModel> employeeDisciplines = disciplineService.GetDisciplineDetail(criteria);

            return employeeDisciplines;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.DisciplineSummaryByPeriod, Policy = Policy.Read)]
        public IEnumerable<DisciplineSummaryByPeriodModel> GetDisciplineSummaryByPeriod(
            int? periodId = null,
            int? departmentId = null,
            bool? isLocal = null,
            int? employeeId = null,
            int? disciplineTypeId = null)
        {
            var criteria = new GetDisciplineSummaryByPeriodCriteria
            {
                PeriodId = periodId,
                DisciplineTypeId = disciplineTypeId,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                EmployeeId = employeeId
            };
            IEnumerable<DisciplineSummaryByPeriodModel> employeeDisciplines = disciplineService.GetDisciplineSummaryByPeriod(criteria);

            return employeeDisciplines;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.DisciplineSummary, Policy = Policy.Read)]
        public IEnumerable<OtherViolationModel> GetOtherViolations(
            int employeeId,
            DateTime startDate,
            DateTime endDate,
            DateTime? workDate = null)
        {
            var criteria = new GetOtherViolationsByEmployeeIdCriteria
            {
                EmployeeId = employeeId,
                WorkDate = workDate,
                StartDate = startDate,
                EndDate = endDate
            };
            IEnumerable<OtherViolationModel> violations = disciplineService.GetOtherViolations(criteria);

            return violations;
        }

        public IEnumerable<DisciplineTypeModel> GetDisciplineType(
            bool includeOtherViolate,
            bool? isActive = null)
        {
            var criteria = new GetDisciplineTypeCriteria
            {
                IncludeOtherViolate = includeOtherViolate,
                IsActive = isActive
            };
            IEnumerable<DisciplineTypeModel> violations = disciplineService.GetDisciplineType(criteria);

            return violations;
        }

        public int GetUserPermissionOnDisciplineSummary()
        {
            int permission = this.SessionService.GetPermission(EPAFunctionKey.DisciplineSummary);
            return permission;
        }
    }
}