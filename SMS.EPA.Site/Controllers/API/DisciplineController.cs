namespace SMS.EPA.Site.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using HRM.UserControl.Client.Core.Enumerables;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model.Discipline;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class DisciplineController : BaseController
    {
        private readonly IDisciplineService disciplineService;

        public DisciplineController() : this(null)
        {
        }

        public DisciplineController(IDisciplineService disciplineService)
        {
            this.disciplineService = disciplineService ?? new DisciplineService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.DisciplineSummary, Policy = Policy.Read)]
        public IEnumerable<DisciplineSummaryModel> GetDisciplineSummary(
            DateTime startDate,
            DateTime endDate,
            string teamIds = null,
            bool? isLocal = null,
            int? employeeId = null,
            int? reasonId = null,
            bool isShowAll = false)
        {
            var criteria = new GetDisciplineSummaryCriteria
            {
                StartDate = startDate,
                EndDate = endDate,
                TeamIds = teamIds,
                IsLocal = isLocal,
                EmployeeId = employeeId,
                ReasonId = reasonId,
                ApproverId = CurrentEmployeeId,
                IsShowAll = isShowAll
            };
            IEnumerable<DisciplineSummaryModel> employeeDisciplines = disciplineService.GetDisciplineSummary(criteria);

            return employeeDisciplines;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.DisciplineSummaryByPeriod, Policy = Policy.Read)]
        public IEnumerable<DisciplineSummaryForOtherSystem> GetDisciplineSummaryForOtherSystem(
            DateTime startDate,
            DateTime endDate,
            string teamIds = null,
            bool? isLocal = null,
            int? employeeId = null,
            int? reasonId = null)
        {
            var criteria = new GetDisciplineSummaryForOtherSystemCriteria
            {
                StartDate = startDate,
                EndDate = endDate,
                TeamIds = teamIds,
                IsLocal = isLocal,
                EmployeeId = employeeId,
                ReasonId = reasonId
            };
            IEnumerable<DisciplineSummaryForOtherSystem> employeeDisciplines = disciplineService.GetDisciplineSummaryForOtherSystem(criteria);

            return employeeDisciplines;
        }
    }
}