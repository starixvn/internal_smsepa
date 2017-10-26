namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using Core.Services;
    using Criteria;
    using Interfaces;
    using Model.Discipline;
    using Repositories;

    public class DisciplineService : BaseService, IDisciplineService
    {
        private readonly IDisciplineSummaryRepository disciplineSummaryRepository;

        public DisciplineService() : this(null)
        {
        }

        public DisciplineService(IDisciplineSummaryRepository disciplineSummaryRepository)
        {
            this.disciplineSummaryRepository = disciplineSummaryRepository ?? new DisciplineSummaryRepository();
        }

        public IEnumerable<DisciplineSummaryModel> GetDisciplineSummary(GetDisciplineSummaryCriteria criteria)
        {
            IEnumerable<DisciplineSummaryModel> employeeDisciplines = new List<DisciplineSummaryModel>();

            if (criteria != null && criteria.IsValid())
            {
                employeeDisciplines = disciplineSummaryRepository.GetDisciplineSummary(criteria);
            }

            return employeeDisciplines;
        }

        public IEnumerable<DisciplineSummaryForOtherSystem> GetDisciplineSummaryForOtherSystem(GetDisciplineSummaryForOtherSystemCriteria criteria)
        {
            IEnumerable<DisciplineSummaryForOtherSystem> employeeDisciplines = new List<DisciplineSummaryForOtherSystem>();

            if (criteria != null && criteria.IsValid())
            {
                employeeDisciplines = disciplineSummaryRepository.GetDisciplineSummaryForOtherSystem(criteria);
            }

            return employeeDisciplines;
        }
    }
}