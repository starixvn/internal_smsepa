namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Model.Discipline;

    public interface IDisciplineSummaryRepository
    {
        IEnumerable<DisciplineSummaryModel> GetDisciplineSummary(ICriteria criteria);

        IEnumerable<DisciplineSummaryByPeriodModel> GetDisciplineSummaryByPeriod(ICriteria criteria);
    }
}