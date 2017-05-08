namespace SMS.EPA.Model.API
{
    using System;

    public class Discipline
    {
        public int EmployeeId { get; set; }

        public DateTime WorkDate { get; set; }

        public string ShiftName { get; set; }

        public DateTime? CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        public decimal? TotalHours { get; set; }

        public int? LateIn { get; set; }

        public int? EarlyOut { get; set; }

        public int Type { get; set; }

        public decimal DisciplineScore { get; set; }

        public string EmployeeComment { get; set; }

        public string ApproverComment { get; set; }

        public bool IsMissingCheckInOut { get; set; }

        public bool IsIgnoredMissingCheckInOut { get; set; }

        public bool IsLateIn { get; set; }

        public bool IsIgnoredLateIn { get; set; }

        public bool IsEarlyOut { get; set; }

        public bool IsIgnoredEarlyOut { get; set; }

        public bool IsWorkingUndertime { get; set; }

        public bool IsIgnoredWorkingUndertime { get; set; }

        public bool IsChangeShift { get; set; }

        public bool IsIgnoredChangeShift { get; set; }

        public bool IsAbsent { get; set; }

        public bool IsIgnoredAbsent { get; set; }

        public bool IsUnpaidLeave { get; set; }

        public bool IsIgnoredUnpaidLeave { get; set; }

        public bool IsUnpaidLeaveAbsent { get; set; }

        public bool IsIgnoredUnpaidLeaveAbsent { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
