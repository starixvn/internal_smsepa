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
    using SMS.EPA.Model.Employee;

    public class RawAttendanceDetailModel
    {
        public RawAttendanceDetailModel()
        {
            Employees = new List<EmployeeModel>();
            Violations = new List<AttendanceDetailModel>();
        }

        public IEnumerable<EmployeeModel> Employees { get; set; }

        public IEnumerable<AttendanceDetailModel> Violations { get; set; }
    }
}
