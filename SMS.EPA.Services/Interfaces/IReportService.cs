namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using System.IO;
    using Criteria.Report;
    using Model;

    public interface IReportService
    {
        DashboardModel GetDashboard(int employeeId);

        EPASummaryModel GetEPASummaryReport(GetEPASummaryCriteria criteria);

        MemoryStream ExportEPASummaryReport(EPASummaryModel epaSummary);

        MemoryStream ExportMonthlyEPASummaryReport(EPASummaryModel epaSummary);

        EPADetailModel GetEPADetailReport(GetEPADetailCriteria criteria);

        PeriodModel GetEPADetailPeriodForExportedFileName(string periodId);

        EPADetailModel GetMonthlyEPADetailReport(GetMonthlyEPADetailCriteria criteria);

        EPASummaryModel GetMonthlyEPASummaryReport(GetMonthlyEPASummaryCriteria criteria);

        IEnumerable<EmployeeViewModel> GetEmployeeByActorId(GetEmployeeByActorIdCriteria criteria);
    }
}