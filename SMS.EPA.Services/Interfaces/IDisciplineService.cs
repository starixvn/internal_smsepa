namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria;
    using Model;

    public interface IDisciplineService
    {
        IEnumerable<DisciplineSummaryModel> GetDisciplineSummary(GetDisciplineSummaryCriteria criteria);

        IEnumerable<OtherViolationModel> GetOtherViolations(GetOtherViolationsByEmployeeIdCriteria criteria);

        IEnumerable<DisciplineTypeModel> GetDisciplineType(GetDisciplineTypeCriteria criteria);

        IEnumerable<DisciplineDetailModel> GetDisciplineDetail(GetDisciplineDetailCriteria criteria);

        IEnumerable<DisciplineSummaryByPeriodModel> GetDisciplineSummaryByPeriod(GetDisciplineSummaryByPeriodCriteria criteria);
    }
}