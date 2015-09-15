// ----------------------------------------------------------------------
// <copyright file="IAttendanceSummaryRepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/09/2015">Created.</change>
// </history>

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
