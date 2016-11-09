namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using System.IO;
    using SMS.EPA.Criteria.Report;
    using SMS.EPA.Model;

    public interface IReportService
    {
        DashboardModel GetDashboard(int employeeId);

        EPASummaryModel GetEPASummaryReport(GetEPASummaryCriteria criteria);

        MemoryStream ExportEPASummaryReport(EPASummaryModel epaSummary);

        MemoryStream ExportMonthlyEPASummaryReport(EPASummaryModel epaSummary);

        EPADetailModel GetEPADetailReport(GetEPADetailCriteria criteria);

        EPADetailModel GetMonthlyEPADetailReport(GetMonthlyEPADetailCriteria criteria);

        EPASummaryModel GetMonthlyEPASummaryReport(GetMonthlyEPASummaryCriteria criteria);

        IEnumerable<EmployeeViewModel> GetEmployeeByActorId(GetEmployeeByActorIdCriteria criteria);
    }
}