namespace SMS.EPA.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.AttendanceSummary;

    public interface IAttendanceSummaryRepository
    {
        IList<AttendanceSummaryModel> GetAttendanceSummary(ICriteria criteria);

        IList<AttendanceDetailModel> GetAttendanceDetail(ICriteria criteria);
    }
}