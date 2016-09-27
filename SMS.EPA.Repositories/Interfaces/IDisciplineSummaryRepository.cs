namespace SMS.EPA.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Model;

    public interface IDisciplineSummaryRepository
    {
        IEnumerable<DisciplineSummaryModel> GetDisciplineSummary(ICriteria criteria);

        IEnumerable<DisciplineDetailModel> GetDisciplineDetail(ICriteria criteria);

        IEnumerable<DisciplineSummaryByPeriodModel> GetDisciplineSummaryByPeriod(ICriteria criteria);
    }
}