namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using Criteria;
    using Interfaces;
    using Model.Discipline;
    using SMS.EPA.Core.Services;

    public class DisciplineService : IDisciplineService
    {
        private readonly IBaseService baseService;

        public DisciplineService() : this(null)
        {
        }

        public DisciplineService(IBaseService baseService)
        {
            this.baseService = baseService ?? new BaseService();
        }

        public IEnumerable<DisciplineSummaryModel> GetDisciplineSummary(GetDisciplineSummaryCriteria criteria)
        {
            return baseService.GetData<DisciplineSummaryModel>(criteria);
        }

        public IEnumerable<DisciplineSummaryForOtherSystem> GetDisciplineSummaryForOtherSystem(GetDisciplineSummaryForOtherSystemCriteria criteria)
        {
            return baseService.GetData<DisciplineSummaryForOtherSystem>(criteria);
        }
    }
}