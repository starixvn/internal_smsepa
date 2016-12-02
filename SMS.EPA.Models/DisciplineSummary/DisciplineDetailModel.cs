namespace SMS.EPA.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Model.Enitites;

    [Serializable]
    public class DisciplineDetailModel
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

        public Employee Employee { get; set; }

        public ViolationModel MissingCheckInOut { get; set; }

        public ViolationModel WorkingUnderTime { get; set; }

        public ViolationModel LateIn { get; set; }

        public ViolationModel EarlyOut { get; set; }

        public ViolationModel Absent { get; set; }

        public ViolationModel UnpaidLeaveAbsent { get; set; }

        public ViolationModel UnpaidLeave { get; set; }

        public ViolationModel ChangeShift { get; set; }

        public decimal SystemSubtractScore
        {
            get
            {
                decimal score = 0;
                var violations = new List<ViolationModel>
                {
                    MissingCheckInOut,
                    LateIn,
                    EarlyOut,
                    WorkingUnderTime,
                    Absent,
                    UnpaidLeave,
                    UnpaidLeaveAbsent,
                    ChangeShift
                };

                foreach (ViolationModel violation in violations.Where(violation => violation != null))
                {
                    score += violation.Value * violation.Score;
                }

                return score;
            }
        }

        public decimal ApproverSubtractScore
        {
            get
            {
                decimal score = 0;
                var violations = new List<ViolationModel>
                {
                    MissingCheckInOut,
                    LateIn,
                    EarlyOut,
                    WorkingUnderTime,
                    Absent,
                    UnpaidLeave,
                    UnpaidLeaveAbsent,
                    ChangeShift
                };

                foreach (ViolationModel violation in violations.Where(violation => violation != null))
                {
                    score += !violation.HasIgnored ? (violation.Value * violation.Score) : 0;
                }

                return score;
            }
        }

        public string ApproverComment { get; set; }

        public string EmployeeComment { get; set; }

        public string WorkDateToString
        {
            get
            {
                return WorkDate.ToString("ddd, dd MMM, yyyy");
            }
        }

        public bool IsWeekend
        {
            get
            {
                return WorkDate.DayOfWeek == DayOfWeek.Saturday || WorkDate.DayOfWeek == DayOfWeek.Sunday;
            }
        }
    }
}