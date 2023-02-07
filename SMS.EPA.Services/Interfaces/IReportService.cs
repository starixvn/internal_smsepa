namespace SMS.EPA.Services
{
    using System.Collections.Generic;
    using System.IO;
    using HRM.UserControl.Client.Core.Models.Role;
    using SMS.EPA.Criteria.Report;
    using SMS.EPA.Model;

    public interface IReportService
    {
        DashboardModel GetDashboard(int employeeId);

        EPASummaryModel GetEPASummaryReport(GetEPASummaryCriteria criteria);

        MemoryStream ExportEPASummaryReport(EPASummaryModel epaSummary);

        MemoryStream ExportMonthlyEPASummaryReport(EPASummaryModel epaSummary);

        EPADetailModel GetEPADetailReport(GetEPADetailCriteria criteria, int currentEmployeeId, IList<Role> roles);

        PeriodModel GetEPADetailPeriodForExportedFileName(string periodId);

        EPADetailModel GetMonthlyEPADetailReport(GetMonthlyEPADetailCriteria criteria, int currentEmployeeId, IList<Role> roles);

        EPASummaryModel GetMonthlyEPASummaryReport(GetMonthlyEPASummaryCriteria criteria);

        IEnumerable<EmployeeViewModel> GetEmployeeByActorId(GetEmployeeByActorIdCriteria criteria);
    }
}