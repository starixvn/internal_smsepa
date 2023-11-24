namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
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

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.DisciplineSummary, Policy = Policy.Read)]
        public IEnumerable<DisciplineSummaryModel> GetDisciplineSummary([FromUri] GetDisciplineSummaryCriteria criteria)
        {
            criteria.ApproverId = CurrentEmployeeId;
            IEnumerable<DisciplineSummaryModel> disciplines = disciplineService.GetDisciplineSummary(criteria);
            return disciplines;
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.DisciplineSummaryByPeriod, Policy = Policy.Read)]
        public IEnumerable<DisciplineSummaryForOtherSystem> GetDisciplineSummaryForOtherSystem([FromUri] GetDisciplineSummaryForOtherSystemCriteria criteria)
        {
            IEnumerable<DisciplineSummaryForOtherSystem> disciplines = disciplineService.GetDisciplineSummaryForOtherSystem(criteria);
            return disciplines;
        }
    }
}