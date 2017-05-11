namespace SMS.EPA.Model.Discipline
{
    using Core.Constants;
    using Enitites;
    using Report;
    using System;

    [Serializable]
    public class DisciplineSummaryModel : DisciplineSummaryBase, IExcelRecord
    {
        public Employee Employee { get; set; }

        public DateTime Period { get; set; }

        public int Permission { get; set; }

        public object ConvertToExcelRecord()
        {
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
                MissingCheckInOut,
                WorkingUndertime,
                LateIn,
                EarlyOut,
                Absent,
                UnpaidLeave,
                UnpaidLeaveAbsent,
                ChangeShift,
                UnplannedLeave,
                WeekendLeave,
                OtherViolation = otherViolations,
                SystemSubtractScore = systemSubtractScore,
                ApproverSubtractScore = approverSubtractScore
            };

            return record;
        }
    }
}