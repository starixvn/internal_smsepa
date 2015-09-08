// ----------------------------------------------------------------------
// <copyright file="AttendanceSummaryModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/08/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.AttendanceSummary
{
    using SMS.EPA.Model.Employee;

    public class AttendanceSummaryModel : EmployeeModel
    {
        public int CheckInOut { get; set; }

        public int WorkingTime { get; set; }

        public int LateIn { get; set; }

        public int EarlyOut { get; set; }

        public int Absent { get; set; }

        public int UnpaidLeave { get; set; }

        public int ChangeShift { get; set; }

        public int SystemSubtractScore { get; set; }

        public int ApproverSubtractScore { get; set; }
    }
}
