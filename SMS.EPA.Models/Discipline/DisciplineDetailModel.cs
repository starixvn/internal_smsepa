namespace SMS.EPA.Model
{
    using Core.Constants;
    using Core.Utilities;
    using Enitites;
    using Report;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [Serializable]
    public class DisciplineDetailModel : IExcelRecord
    {
        public int EmployeeId { get; set; }

        public DateTime WorkDate { get; set; }

        public string WorkDateToDisplay => WorkDate.ToString("ddd, dd MMM, yyyy");

        public string ShiftName { get; set; }

        public bool OutOfDate { get; set; }

        public DateTime? CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        public string TotalHours { get; set; }

        public string NumberOfMinutesLateIn { get; set; }

        public string NumberOfMinutesEarlyOut { get; set; }

        public Employee Employee { get; set; }

        public DisciplineModel MissingCheckInOut { get; set; }

        public DisciplineModel WorkingUnderTime { get; set; }

        public DisciplineModel LateIn { get; set; }

        public DisciplineModel EarlyOut { get; set; }

        public DisciplineModel Absent { get; set; }

        public DisciplineModel UnpaidLeaveAbsent { get; set; }

        public DisciplineModel UnpaidLeave { get; set; }

        public DisciplineModel ChangeShift { get; set; }

        public DisciplineModel UnplannedLeave { get; set; }

        public DisciplineModel WeekendLeave { get; set; }

        public decimal SystemSubtractScore
        {
            get
            {
                decimal score = 0;
                var violations = new List<DisciplineModel>
                {
                    MissingCheckInOut,
                    LateIn,
                    EarlyOut,
                    WorkingUnderTime,
                    Absent,
                    UnpaidLeave,
                    UnpaidLeaveAbsent,
                    ChangeShift,
                    UnplannedLeave,
                    WeekendLeave
                };

                foreach (DisciplineModel violation in violations.Where(violation => violation != null))
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
                var violations = new List<DisciplineModel>
                {
                    MissingCheckInOut,
                    LateIn,
                    EarlyOut,
                    WorkingUnderTime,
                    Absent,
                    UnpaidLeave,
                    UnpaidLeaveAbsent,
                    ChangeShift,
                    UnplannedLeave,
                    WeekendLeave
                };

                foreach (DisciplineModel violation in violations.Where(violation => violation != null))
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
                Date = WorkDateToDisplay,
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
                UnplannedLeave = ExcelRecordDataConverter.ZeroValueToEmptyStringConverter(UnplannedLeave.Value),
                WeekendLeave = ExcelRecordDataConverter.ZeroValueToEmptyStringConverter(WeekendLeave.Value),
                SystemSubtractScore = SystemSubtractScore > 0 ? -SystemSubtractScore : 0,
                ApproverSubtractScore = ApproverSubtractScore > 0 ? -ApproverSubtractScore : 0,
                ApproverComment,
                EmployeeComment
            };

            return record;
        }
    }
}