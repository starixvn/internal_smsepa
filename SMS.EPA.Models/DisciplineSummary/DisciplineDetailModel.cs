namespace SMS.EPA.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Constants;
    using Core.Utilities;
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

        public bool IsWeekend => WorkDate.DayOfWeek == DayOfWeek.Saturday || WorkDate.DayOfWeek == DayOfWeek.Sunday;

        public object ConvertToExcelRecord()
        {
            string section = Employee.Group != null ? Employee.Group.GroupName : string.Empty;
            string jobTitle = Employee.JobTitle != null ? Employee.JobTitle.JobTitleName : string.Empty;

            var record = new
            {
                Shift = Employee.Code,
                Nick = Employee.NickName,
                Employee.FullName,
                Section = section,
                JobTitle = jobTitle,
                JoinedDate = Employee.JoinedDate.ToString(Constants.DateFormat),
                Employee.Category,
                Date = WorkDate.ToString("ddd, dd MMM, yyyy"),
                Code = ShiftName,
                CheckIn = ExcelRecordDataConverter.NullabeDateTimeToStringConverter(CheckIn),
                CheckOut = ExcelRecordDataConverter.NullabeDateTimeToStringConverter(CheckOut),
                TotalHours = ExcelRecordDataConverter.NullToEmptyStringConverter(TotalHours),
                MissingCheckInOut = ExcelRecordDataConverter.ZeroValueToEmptyStringConverter(MissingCheckInOut.Value),
                WorkingUnderTime = ExcelRecordDataConverter.ValueToOneStringConverter(WorkingUnderTime.Value),
                LateIn = ExcelRecordDataConverter.NullToEmptyStringConverter(NumberOfMinutesLateIn),
                EarlyOut = ExcelRecordDataConverter.NullToEmptyStringConverter(NumberOfMinutesEarlyOut),
                Absent = ExcelRecordDataConverter.ZeroValueToEmptyStringConverter(Absent.Value),
                UnpaidLeave = ExcelRecordDataConverter.ZeroValueToEmptyStringConverter(UnpaidLeave.Value),
                UnpaidLeaveAbsent = ExcelRecordDataConverter.ZeroValueToEmptyStringConverter(UnpaidLeaveAbsent.Value),
                ChangeShift = ExcelRecordDataConverter.ZeroValueToEmptyStringConverter(ChangeShift.Value),
                SystemSubtractScore = SystemSubtractScore > 0 ? -SystemSubtractScore : 0,
                ApproverSubtractScore = ApproverSubtractScore > 0 ? -ApproverSubtractScore : 0,
                ApproverComment,
                EmployeeComment
            };

            return record;
        }
    }
}