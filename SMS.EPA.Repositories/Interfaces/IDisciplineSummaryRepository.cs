namespace SMS.EPA.Repositories
{
    using Fanex.Data.Repository;
    using Model;
    using System.Collections.Generic;

    public interface IDisciplineSummaryRepository
    {
        IEnumerable<DisciplineSummaryModel> GetDisciplineSummary(ICriteria criteria);

        IEnumerable<DisciplineDetailModel> GetDisciplineDetail(ICriteria criteria);

        IEnumerable<DisciplineSummaryByPeriodModel> GetDisciplineSummaryByPeriod(ICriteria criteria);
    }
}