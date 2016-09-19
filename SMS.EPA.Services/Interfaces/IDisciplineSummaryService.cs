namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;

    public interface IDisciplineSummaryService
    {
        IEnumerable<DisciplineSummaryModel> GetDisciplineSummaryByPeriod(GetDisciplineSummaryByPeriodCriteria criteria);

        void ExportDisciplineSummaryByPeriodReport(
            GetDisciplineSummaryByPeriodCriteria criteria,
            IEnumerable<DisciplineSummaryModel> employeeDisciplines,
            string templatePath);
    }
}