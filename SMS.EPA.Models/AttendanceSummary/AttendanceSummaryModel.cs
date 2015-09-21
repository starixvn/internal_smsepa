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
        public int MissingCheckInOut { get; set; }

        public int WorkingBelowTime { get; set; }

        public int LateIn { get; set; }

        public int EarlyOut { get; set; }

        public int Absent { get; set; }

        public int UnpaidLeave { get; set; }

        public int UnpaidLeaveAbsent { get; set; }

        public int ChangeShift { get; set; }

        public float SystemSubtractScore { get; set; }

        public float ApproverIgnoredScore { get; set; }

        public float ApproverSubtractScore 
        {
            get
            {
                return SystemSubtractScore - ApproverIgnoredScore;
            }

            set
            {
                ApproverSubtractScore = value;
            }
        }

        public float OtherViolationsScore { get; set; }
    }
}
