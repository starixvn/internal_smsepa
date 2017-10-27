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
        private readonly IDisciplineRepository disciplineRepository;

        public DisciplineService() : this(null)
        {
        }

        public DisciplineService(IDisciplineRepository disciplineRepository)
        {
            this.disciplineRepository = disciplineRepository ?? new DisciplineRepository();
        }

        public IEnumerable<DisciplineSummaryModel> GetDisciplineSummary(GetDisciplineSummaryCriteria criteria)
        {
            IEnumerable<DisciplineSummaryModel> employeeDisciplines = new List<DisciplineSummaryModel>();

            if (criteria != null && criteria.IsValid())
            {
                employeeDisciplines = disciplineRepository.GetDisciplineSummary(criteria);
            }

            return employeeDisciplines;
        }

        public IEnumerable<DisciplineSummaryForOtherSystem> GetDisciplineSummaryForOtherSystem(GetDisciplineSummaryForOtherSystemCriteria criteria)
        {
            IEnumerable<DisciplineSummaryForOtherSystem> employeeDisciplines = new List<DisciplineSummaryForOtherSystem>();

            if (criteria != null && criteria.IsValid())
            {
                employeeDisciplines = disciplineRepository.GetDisciplineSummaryForOtherSystem(criteria);
            }

            return employeeDisciplines;
        }
    }
}