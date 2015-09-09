// ----------------------------------------------------------------------
// <copyright file="RawAttendanceSummaryModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/09/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.AttendanceSummary
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Employee;

    public class RawAttendanceSummaryModel
    {
        public IEnumerable<EmployeeModel> Employees { get; set; }

        public IEnumerable<AttendanceSummaryModel> EmployeeViolations { get; set; }
    }
}
