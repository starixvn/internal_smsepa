namespace SMS.EPA.Model.Discipline
{
    using System;
    using Core.Constants;
    using Report;

    public class DisciplineSummaryForOtherSystem : DisciplineSummaryBase, IExcelRecord
    {
        public int EmployeeId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Category { get; set; }
        public DateTime JoinedDate { get; set; }
        public string Section { get; set; }
        public string JobTitle { get; set; }

        public object ConvertToExcelRecord()
        {
            decimal missingCheckInOut = MissingCheckInOut > 0 ? -MissingCheckInOut : 0;
            decimal workingUndertime = WorkingUndertime > 0 ? -WorkingUndertime : 0;
            decimal lateIn = LateIn > 0 ? -LateIn : 0;
            decimal earlyOut = EarlyOut > 0 ? -EarlyOut : 0;
            decimal absent = Absent > 0 ? -Absent : 0;
            decimal unpaidLeave = UnpaidLeave > 0 ? -UnpaidLeave : 0;
            decimal unpaidLeaveAbsent = UnpaidLeaveAbsent > 0 ? -UnpaidLeaveAbsent : 0;
            decimal changeShift = ChangeShift > 0 ? -ChangeShift : 0;
            decimal unplannedLeave = UnplannedLeave > 0 ? -UnplannedLeave : 0;
            decimal weekendLeave = WeekendLeave > 0 ? -WeekendLeave : 0;
            decimal sevenDaysLeave = SevenDaysLeave > 0 ? -SevenDaysLeave : 0;
            decimal otherDisciplines = OtherDisciplines > 0 ? -OtherDisciplines : 0;
            decimal approverSubtractScore = ApproverSubtractScore > 0 ? -ApproverSubtractScore : 0;

            var record = new
            {
                Count = 0,
                Code,
                Nickname,
                FullName,
                Section,
                JobTitle,
                JoinedDate = JoinedDate.ToString(Constants.DateFormat),
                Category,
                MissingCheckInOut = missingCheckInOut,
                WorkingUndertime = workingUndertime,
                LateIn = lateIn,
                EarlyOut = earlyOut,
                Absent = absent,
                UnpaidLeave = unpaidLeave,
                UnpaidLeaveAbsent = unpaidLeaveAbsent,
                ChangeShift = changeShift,
                UnplannedLeave = unplannedLeave,
                WeekendLeave = weekendLeave,
                SevenDaysLeave = sevenDaysLeave,
                OtherDisciplines = otherDisciplines,
                ApproverSubtractScore = approverSubtractScore
            };

            return record;
        }
    }
}