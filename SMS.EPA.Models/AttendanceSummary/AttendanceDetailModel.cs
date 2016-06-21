namespace SMS.EPA.Model.AttendanceSummary
{
    using System;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.Enitites;

    [Serializable]
    public class AttendanceDetailModel
    {
        public int EmployeeId { get; set; }

        public DateTime WorkDate { get; set; }

        public string ShiftName { get; set; }

        public bool OutOfDate { get; set; }

        public string InFixed { get; set; }

        public string OutFixed { get; set; }

        public EmployeeModel Employee { get; set; }

        public JobTitle JobTitle { get; set; }

        public ViolationModel MissingCheckInOut { get; set; }

        public ViolationModel TotalHours { get; set; }

        public ViolationModel LateIn { get; set; }

        public ViolationModel EarlyOut { get; set; }

        public ViolationModel Absent { get; set; }

        public ViolationModel UnpaidLeaveAbsent { get; set; }

        public ViolationModel UnpaidLeave { get; set; }

        public ViolationModel ChangeShift { get; set; }

        public float SystemSubtractScore { get; set; }

        public float ApproverSubtractScore { get; set; }

        public string ApproverComment { get; set; }

        public string EmployeeComment { get; set; }
    }
}