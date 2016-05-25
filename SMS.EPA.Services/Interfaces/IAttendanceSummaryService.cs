namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Criteria.AttendanceSummary;
    using SMS.EPA.Model.AttendanceSummary;

    public interface IAttendanceSummaryService
    {
        IEnumerable<AttendanceSummaryModel> GetAttendanceSummary(GetAttendanceSummaryCriteria criteria);

        IEnumerable<OtherViolationModel> GetOtherViolations(GetOtherViolationsOfEmployeeCriteria criteria);

        IEnumerable<ViolationModel> GetViolationType(GetViolationTypeCriteria criteria);

        IEnumerable<AttendanceDetailModel> GetAttendanceDetail(GetDetailAttendanceCriteria criteria);

        IEnumerable<AttendanceScoreModel> GetAttendanceScoreByEPAId(GetAttendanceScoreByEPAIdCriteria criteria);

        void ExportAttendanceSummaryReport(GetAttendanceSummaryCriteria criteria, IEnumerable<AttendanceSummaryModel> attendanceEmployees, string templatePath);

        void ExportAttendanceDetailReport(GetDetailAttendanceCriteria criteria, IEnumerable<AttendanceDetailModel> attendanceViolations, string templatePath);

        IList<OtherViolationTypeModel> GetOtherViolationType(GetOtherViolationTypeCriteria criteria);
    }
}