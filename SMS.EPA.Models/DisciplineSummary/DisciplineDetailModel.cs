namespace SMS.EPA.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Constants;
    using Enitites;
    using Report;

    [Serializable]
    public class DisciplineDetailModel : IExcelRecord
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

        public string WorkDateToString => WorkDate.ToString("ddd, dd MMM, yyyy");

        public bool IsWeekend => WorkDate.DayOfWeek == DayOfWeek.Saturday || WorkDate.DayOfWeek == DayOfWeek.Sunday;

        public object ConvertToExcelRecord()
        {
            var record = new
            {
                Shift = Employee.Code,
                Nick = Employee.NickName,
                FullName = Employee.FullName,
                Section = Employee.Group != null ? Employee.Group.GroupName : string.Empty,
                JobTitle = Employee.JobTitle != null ? Employee.JobTitle.JobTitleName : string.Empty,
                JoinedDate = Employee.JoinedDate.ToString(Constants.DateFormat),
                Category = Employee.Category,
                Date = WorkDateToString,
                Code = ShiftName,
                CheckIn = CheckIn?.ToString(Constants.TimeFormat) ?? string.Empty,
                CheckOut = CheckOut?.ToString(Constants.TimeFormat) ?? string.Empty,
                TotalHours = !string.IsNullOrEmpty(TotalHours) ? TotalHours : string.Empty,
                MissingCheckInOut = MissingCheckInOut.Value > 0 ? MissingCheckInOut.Value.ToString() : string.Empty,
                WorkingUnderTime = WorkingUnderTime.Value > 0 ? "1" : string.Empty,
                LateIn = !string.IsNullOrEmpty(NumberOfMinutesLateIn) ? NumberOfMinutesLateIn : string.Empty,
                EarlyOut = !string.IsNullOrEmpty(NumberOfMinutesEarlyOut) ? NumberOfMinutesEarlyOut : string.Empty,
                Absent = Absent.Value > 0 ? Absent.Value.ToString() : string.Empty,
                UnpaidLeave = UnpaidLeave.Value > 0 ? UnpaidLeave.Value.ToString() : string.Empty,
                UnpaidLeaveAbsent = UnpaidLeaveAbsent.Value > 0 ? UnpaidLeaveAbsent.Value.ToString() : string.Empty,
                ChangeShift = ChangeShift.Value > 0 ? ChangeShift.Value.ToString() : string.Empty,
                SystemSubtractScore = SystemSubtractScore > 0 ? -SystemSubtractScore : 0,
                ApproverSubtractScore = ApproverSubtractScore > 0 ? -ApproverSubtractScore : 0,
                ApproverComment = ApproverComment,
                EmployeeComment = EmployeeComment
            };

            return record;
        }
    }
}