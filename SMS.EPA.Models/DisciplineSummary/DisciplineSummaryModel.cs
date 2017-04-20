namespace SMS.EPA.Model
{
    using System;
    using Core.Constants;
    using Enitites;
    using Report;

    [Serializable]
    public class DisciplineSummaryModel : IExcelRecord
    {
        public Employee Employee { get; set; }

        public int MissingCheckInOut { get; set; }

        public int WorkingUnderTime { get; set; }

        public int LateIn { get; set; }

        public int EarlyOut { get; set; }

        public int Absent { get; set; }

        public int UnpaidLeave { get; set; }

        public int UnpaidLeaveAbsent { get; set; }

        public int ChangeShift { get; set; }

        public float OtherViolationsScore { get; set; }

        public float SystemSubtractScore { get; set; }

        public float ApproverIgnoredScore { get; set; }

        public float ApproverSubtractScore { get; set; }

        public int Permission { get; set; }

        public object ConvertToExcelRecord()
        {
            float otherViolations = OtherViolationsScore > 0
                    ? -OtherViolationsScore
                    : 0;
            float systemSubtractScore = SystemSubtractScore > 0
                    ? -SystemSubtractScore
                    : 0;
            float approverSubtractScore = ApproverSubtractScore > 0
                    ? -ApproverSubtractScore
                    : 0;

            var record = new
            {
                Count = 0,
                Section = Employee.Group != null
                    ? Employee.Group.GroupName
                    : string.Empty,
                Code = Employee.Code,
                Nick = Employee.NickName,
                FullName = Employee.FullName,
                JobTitle = Employee.JobTitle != null
                    ? Employee.JobTitle.JobTitleName
                    : string.Empty,
                JoinedDate = Employee.JoinedDate.ToString(Constants.DateFormat),
                Category = Employee.Category,
                MissingCheckInOut = MissingCheckInOut,
                WorkingUnderTime = WorkingUnderTime,
                LateIn = LateIn,
                EarlyOut = EarlyOut,
                Absent = Absent,
                UnpaidLeave = UnpaidLeave,
                UnpaidLeaveAbsent = UnpaidLeaveAbsent,
                ChangeShift = ChangeShift,
                OtherViolation = (decimal) otherViolations,
                SystemSubtractScore = (decimal) systemSubtractScore,
                ApproverSubtractScore = (decimal) approverSubtractScore
            };

            return record;
        }
    }
}