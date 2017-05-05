namespace SMS.EPA.Model
{
    using Core.Constants;
    using Enitites;
    using Report;

    public class DisciplineSummaryByPeriodModel : IExcelRecord
    {
        public Employee Employee { get; set; }

        public PeriodModel Period { get; set; }

        public decimal ChangeShift { get; set; }

        public decimal MissingCheckInOut { get; set; }

        public decimal LateIn { get; set; }

        public decimal EarlyOut { get; set; }

        public decimal Absent { get; set; }

        public decimal UnpaidLeave { get; set; }

        public decimal UnplannedLeave { get; set; }

        public decimal WeekendLeave { get; set; }

        public decimal WorkingUndertime { get; set; }

        public decimal UnpaidLeaveAbsent { get; set; }

        public decimal OtherDisciplines { get; set; }

        public decimal SystemSubtractScore { get; set; }

        public decimal ApproverSubtractScore
        {
            get
            {
                decimal total = ChangeShift + MissingCheckInOut + LateIn + EarlyOut + Absent + UnpaidLeave +
                                UnplannedLeave + WeekendLeave + WorkingUndertime + UnpaidLeaveAbsent + OtherDisciplines;
                return total;
            }
        }

        public object ConvertToExcelRecord()
        {
            decimal missingCheckInOut = MissingCheckInOut > 0 ? -MissingCheckInOut : 0;
            decimal lateIn = LateIn > 0 ? -LateIn : 0;
            decimal unpaidLeave = UnpaidLeave > 0 ? -UnpaidLeave : 0;
            decimal workingUndertime = WorkingUndertime > 0 ? -WorkingUndertime : 0;
            decimal otherDisciplines = OtherDisciplines > 0 ? -OtherDisciplines : 0;
            decimal approverSubtractScore = ApproverSubtractScore > 0 ? -ApproverSubtractScore : 0;

            var record = new
            {
                Count = 0,
                Period = Period.Name,
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
                WorkingUndertime = workingUndertime,
                OtherDisciplines = otherDisciplines,
                ApproverSubtractScore = approverSubtractScore
            };

            return record;
        }
    }
}