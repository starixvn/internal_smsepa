namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;

    public interface IDisciplineSummaryService
    {
        IEnumerable<DisciplineSummaryModel> GetDisciplineSummary(GetDisciplineSummaryCriteria criteria);

        IEnumerable<OtherViolationModel> GetOtherViolations(GetOtherViolationsByEmployeeIdCriteria criteria);

        IEnumerable<DisciplineTypeModel> GetDisciplineType(GetDisciplineTypeCriteria criteria);

        IEnumerable<DisciplineDetailModel> GetDisciplineDetail(GetDisciplineDetailCriteria criteria);

        IEnumerable<DisciplineSummaryByPeriodModel> GetDisciplineSummaryByPeriod(GetDisciplineSummaryByPeriodCriteria criteria);

        void ExportDisciplineSummaryReport(
            GetDisciplineSummaryCriteria criteria,
            IEnumerable<DisciplineSummaryModel> employeeDisciplines,
            string templatePath);

        void ExportDisciplineDetailReport(
            GetDisciplineDetailCriteria criteria,
            IEnumerable<DisciplineDetailModel> employeeDisciplines,
            string templatePath);

        void ExportDisciplineSummaryByPeriodReport(
            GetDisciplineSummaryByPeriodCriteria criteria,
            IEnumerable<DisciplineSummaryByPeriodModel> employeeDisciplines,
            string templatePath);
    }
}