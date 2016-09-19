namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;

    public interface IAttendanceSummaryService
    {
        IEnumerable<AttendanceSummaryModel> GetAttendanceSummary(GetAttendanceSummaryCriteria criteria);

        IEnumerable<OtherViolationModel> GetOtherViolations(GetOtherViolationsOfEmployeeCriteria criteria);

        IEnumerable<ViolationTypeModel> GetViolationType(GetViolationTypeCriteria criteria);

        IEnumerable<AttendanceDetailModel> GetAttendanceDetail(GetDetailAttendanceCriteria criteria);

        void ExportAttendanceSummaryReport(GetAttendanceSummaryCriteria criteria, IEnumerable<AttendanceSummaryModel> attendanceEmployees, string templatePath);

        void ExportAttendanceDetailReport(GetDetailAttendanceCriteria criteria, IEnumerable<AttendanceDetailModel> attendanceViolations, string templatePath);
    }
}