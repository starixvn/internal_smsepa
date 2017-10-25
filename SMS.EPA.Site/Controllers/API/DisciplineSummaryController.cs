namespace SMS.EPA.Site.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using Core.Attribute;
    using Core.Constants;
    using Core.Enumerations;
    using Criteria;
    using Model;
    using Model.Discipline;
    using Services.Implement;
    using Services.Interfaces;

    public class DisciplineSummaryController : BaseController
    {
        private readonly IDisciplineService disciplineService;

        public DisciplineSummaryController() : this(null)
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
                ApproverId = SessionService.EmployeeId,
                IsShowAll = isShowAll
            };
            IEnumerable<DisciplineSummaryModel> employeeDisciplines = disciplineService.GetDisciplineSummary(criteria);

            return employeeDisciplines;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.DisciplineSummaryByPeriod, Policy = Policy.Read)]
        public IEnumerable<DisciplineSummaryForOtherSystem> GetDisciplineSummaryForOtherSystem(
            DateTime startDate,
            DateTime endDate,
            int? departmentId = null,
            bool? isLocal = null,
            int? employeeId = null,
            int? reasonId = null)
        {
            var criteria = new GetDisciplineSummaryForOtherSystemCriteria
            {
                StartDate = startDate,
                EndDate = endDate,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                EmployeeId = employeeId,
                ReasonId = reasonId
            };
            IEnumerable<DisciplineSummaryForOtherSystem> employeeDisciplines = disciplineService.GetDisciplineSummaryForOtherSystem(criteria);

            return employeeDisciplines;
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
    }
}