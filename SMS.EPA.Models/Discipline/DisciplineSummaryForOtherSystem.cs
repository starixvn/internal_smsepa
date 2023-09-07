namespace SMS.EPA.Model.Discipline
{
    using System;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Model.Report;

    public class DisciplineSummaryForOtherSystem : DisciplineSummaryBase, IExcelRecord
    {
        public int EmployeeId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Category { get; set; }
        public DateTime JoinedDate { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string Team { get; set; }
        public string JobTitle { get; set; }

        public object ConvertToExcelRecord()
        {
            var record = new
            {
                Count = 0,
                Code,
                Nickname,
                FullName,
                Department,
                Section,
                Team,
                JobTitle,
                JoinedDate = JoinedDate.ToString(Constants.DateFormat),
                Category,
                MissingCheckInOut = GetScore(MissingCheckInOut),
                WorkingUndertime = GetScore(WorkingUndertime),
                LateIn = GetScore(LateIn),
                EarlyOut = GetScore(EarlyOut),
                Absent = GetScore(Absent),
                UnpaidLeave = GetScore(UnpaidLeave),
                UnpaidLeaveAbsent = GetScore(UnpaidLeaveAbsent),
                ChangeShift = GetScore(ChangeShift),
                UnplannedLeave = GetScore(UnplannedLeave),
                WeekendLeave = GetScore(WeekendLeave),
                SevenDaysLeave = GetScore(SevenDaysLeave),
                OtherDisciplines = GetScore(OtherDisciplines),
                ApproverSubtractScore = GetScore(ApproverSubtractScore)
            };

            return record;
        }
    }
}