namespace SMS.EPA.Site.Controllers.API
{
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
        private readonly IDisciplineSummaryService disciplineSummaryService;

        public DisciplineSummaryController()
            : this(null)
        {
        }

        public DisciplineSummaryController(IDisciplineSummaryService disciplineSummaryService)
        {
            this.disciplineSummaryService = disciplineSummaryService ?? new DisciplineSummaryService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.DisciplineSummaryByPeriod, Policy = Policy.Read)]
        public IEnumerable<DisciplineSummaryModel> GetDisciplineSummaryByPeriod(
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
            IEnumerable<DisciplineSummaryModel> employeeDisciplines = disciplineSummaryService.GetDisciplineSummaryByPeriod(criteria);

            return employeeDisciplines;
        }
    }
}