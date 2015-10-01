// ----------------------------------------------------------------------
// <copyright file="IAttendanceSummaryService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/08/2015">Created.</change>
// </history>

namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.AttendanceSummary;
    using SMS.EPA.Model.AttendanceSummary;

    public interface IAttendanceSummaryService
    {
        IEnumerable<AttendanceSummaryModel> GetAttendanceSummary(GetAttendanceSummaryCriteria criteria);

        IEnumerable<OtherViolationModel> GetOtherViolations(GetOtherViolationsOfEmployeeCriteria criteria);

        CommandResult InsertOtherViolation(InsertOtherViolationsCriteria criteria);

        CommandResult DeleteOtherViolation(DeleteOtherViolationCriteria criteria);

        IEnumerable<ViolationModel> GetViolationType(GetViolationTypeCriteria criteria);

        IEnumerable<AttendanceDetailModel> GetAttendanceDetail(GetDetailAttendanceCriteria criteria);

        CommandResult UpdateIgnoredViolation(UpdateIgnoredViolationCriteria criteria);

        IEnumerable<AttendanceScoreModel> GetAttendanceScoreByEPAId(GetAttendanceScoreByEPAIdCriteria criteria);
    }
}
