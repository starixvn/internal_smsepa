// ----------------------------------------------------------------------
// <copyright file="RawAttendanceDetailModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/14/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.AttendanceSummary
{
    using System.Collections.Generic;

    public class RawAttendanceDetailModel
    {
        public IEnumerable<AttendanceDetailModel> FirstViolations { get; set; }

        public IEnumerable<AttendanceDetailModel> LastViolations { get; set; }
    }
}
