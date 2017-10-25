namespace SMS.EPA.Model.Discipline
{
    using Core.Constants;
    using Enitites;
    using Report;

    public class DisciplineSummaryForOtherSystem : DisciplineSummaryBase, IExcelRecord
    {
        public Employee Employee { get; set; }

        public object ConvertToExcelRecord()
        {
            decimal missingCheckInOut = MissingCheckInOut > 0 ? -MissingCheckInOut : 0;
            decimal lateIn = LateIn > 0 ? -LateIn : 0;
            decimal unpaidLeave = UnpaidLeave > 0 ? -UnpaidLeave : 0;
            decimal unplannedLeave = UnplannedLeave > 0 ? -UnplannedLeave : 0;
            decimal weekendLeave = WeekendLeave > 0 ? -WeekendLeave : 0;
            decimal workingUndertime = WorkingUndertime > 0 ? -WorkingUndertime : 0;
            decimal otherDisciplines = OtherDisciplines > 0 ? -OtherDisciplines : 0;
            decimal approverSubtractScore = ApproverSubtractScore > 0 ? -ApproverSubtractScore : 0;

            var record = new
            {
                Count = 0,
                Code = Employee.Code,
                Nick = Employee.NickName,
                FullName = Employee.FullName,
                Section = Employee.Group != null ? Employee.Group.GroupName : string.Empty,
                JobTitle = Employee.JobTitle != null ? Employee.JobTitle.JobTitleName : string.Empty,
                JoinedDate = Employee.JoinedDate.ToString(Constants.DateFormat),
                Category = Employee.Category,
                MissingCheckInOut = missingCheckInOut,
                LateIn = lateIn,
                UnpaidLeave = unpaidLeave,
                UnplannedLeave = unplannedLeave,
                WeekendLeave = weekendLeave,
                WorkingUndertime = workingUndertime,
                OtherDisciplines = otherDisciplines,
                ApproverSubtractScore = approverSubtractScore
            };

            return record;
        }
    }
}