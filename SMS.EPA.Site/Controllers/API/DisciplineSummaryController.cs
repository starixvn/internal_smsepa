namespace SMS.EPA.Site.Controllers.API
{
    using Core.Attribute;
    using Core.Constants;
    using Core.Enumerations;
    using Criteria;
    using Model;
    using Services.Implement;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using Model.Discipline;

    public class DisciplineSummaryController : BaseController
    {
        private readonly IDisciplineService _disciplineService;

        public DisciplineSummaryController()
            : this(null)
        {
        }

        public DisciplineSummaryController(IDisciplineService disciplineService)
        {
            _disciplineService = disciplineService ?? new DisciplineService();
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
                ApproverId = SessionService.EmployeeId,
                IsShowAll = isShowAll
            };
            IEnumerable<DisciplineSummaryModel> employeeDisciplines = _disciplineService.GetDisciplineSummary(criteria);

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
                ApproverId = SessionService.EmployeeId,
                HasIgnored = hasIgnored
            };
            IEnumerable<DisciplineDetailModel> employeeDisciplines = _disciplineService.GetDisciplineDetail(criteria);

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
            IEnumerable<DisciplineSummaryByPeriodModel> employeeDisciplines =
                _disciplineService.GetDisciplineSummaryByPeriod(criteria);

            return employeeDisciplines;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.DisciplineSummary, Policy = Policy.Read)]
        public IEnumerable<OtherDisciplineModel> GetOtherViolations(
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
            IEnumerable<OtherDisciplineModel> violations = _disciplineService.GetOtherViolations(criteria);

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
            IEnumerable<DisciplineTypeModel> violations = _disciplineService.GetDisciplineType(criteria);

            return violations;
        }

        public int GetUserPermissionOnDisciplineSummary()
        {
            int permission = SessionService.GetPermission(EPAFunctionKey.DisciplineSummary);
            return permission;
        }
    }
}