namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Model.Discipline;

    public interface IDisciplineRepository
    {
        IEnumerable<DisciplineSummaryModel> GetDisciplineSummary(ICriteria criteria);

        IEnumerable<DisciplineSummaryForOtherSystem> GetDisciplineSummaryForOtherSystem(ICriteria criteria);
    }
}