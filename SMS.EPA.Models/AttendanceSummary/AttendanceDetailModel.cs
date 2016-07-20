namespace SMS.EPA.Model.AttendanceSummary
{
    using System;
    using SMS.EPA.Model.Employee;

    [Serializable]
    public class AttendanceDetailModel
    {
        public int EmployeeId { get; set; }

        public DateTime WorkDate { get; set; }

        public string ShiftName { get; set; }

        public bool OutOfDate { get; set; }

        public DateTime? CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        public string TotalHours { get; set; }

        public string NumberOfMinutesLateIn { get; set; }

        public string NumberOfMinutesEarlyOut { get; set; }

        public EmployeeModel Employee { get; set; }

        public ViolationModel MissingCheckInOut { get; set; }

        public ViolationModel WorkingUnderTime { get; set; }

        public ViolationModel LateIn { get; set; }

        public ViolationModel EarlyOut { get; set; }

        public ViolationModel Absent { get; set; }

        public ViolationModel UnpaidLeaveAbsent { get; set; }

        public ViolationModel UnpaidLeave { get; set; }

        public ViolationModel ChangeShift { get; set; }

        public decimal SystemSubtractScore { get; set; }

        public decimal ApproverSubtractScore { get; set; }

        public string ApproverComment { get; set; }

        public string EmployeeComment { get; set; }
    }
}