namespace SMS.EPA.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.AttendanceSummary;

    public interface IAttendanceSummaryRepository
    {
        IEnumerable<AttendanceSummaryModel> GetAttendanceSummary(ICriteria criteria);

        RawAttendanceDetailModel GetAttendanceDetail(ICriteria criteria);
    }
}