namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria;
    using Model;
    using Model.Discipline;

    public interface IDisciplineService
    {
        IEnumerable<DisciplineSummaryModel> GetDisciplineSummary(GetDisciplineSummaryCriteria criteria);

        IEnumerable<DisciplineTypeModel> GetDisciplineType(GetDisciplineTypeCriteria criteria);

        IEnumerable<DisciplineSummaryByPeriodModel> GetDisciplineSummaryByPeriod(GetDisciplineSummaryByPeriodCriteria criteria);
    }
}