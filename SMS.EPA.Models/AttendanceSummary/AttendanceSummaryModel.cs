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
    using System;
    using SMS.EPA.Model.Enitites;

    [Serializable]
    public class AttendanceSummaryModel
    {
        public Employee Employee { get; set; }

        public JobTitle JobTitle { get; set; }

        public Group Group { get; set; }

        public int MissingCheckInOut { get; set; }

        public int WorkingBelowTime { get; set; }

        public int LateIn { get; set; }

        public int EarlyOut { get; set; }

        public int Absent { get; set; }

        public int UnpaidLeave { get; set; }

        public int UnpaidLeaveAbsent { get; set; }

        public int ChangeShift { get; set; }

        public float OtherViolationsScore { get; set; }

        public float SystemSubtractScore { get; set; }

        public float ApproverIgnoredScore { get; set; }

        public float ApproverSubtractScore { get; set; }

        public int Permission { get; set; }
    }
}
