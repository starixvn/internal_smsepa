namespace SMS.EPA.Model.Discipline
{
    using System;
    using Core.Constants;
    using Enitites;
    using Report;

    [Serializable]
    public class DisciplineSummaryModel : DisciplineSummaryBase, IExcelRecord
    {
        public Employee Employee { get; set; }

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
            decimal otherViolations = OtherDisciplines > 0 ? -OtherDisciplines : 0;
            decimal systemSubtractScore = SystemSubtractScore > 0 ? -SystemSubtractScore : 0;
            decimal approverSubtractScore = ApproverSubtractScore > 0 ? -ApproverSubtractScore : 0;

            var record = new
            {
                Count = 0,
                Section = Employee.Group != null ? Employee.Group.GroupName : string.Empty,
                Employee.Code,
                Nick = Employee.NickName,
                Employee.FullName,
                JobTitle = Employee.JobTitle != null ? Employee.JobTitle.JobTitleName : string.Empty,
                JoinedDate = Employee.JoinedDate.ToString(Constants.DateFormat),
                Employee.Category,
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
                OtherViolation = otherViolations,
                SystemSubtractScore = systemSubtractScore,
                ApproverSubtractScore = approverSubtractScore
            };

            return record;
        }
    }
}