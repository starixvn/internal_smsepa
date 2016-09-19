namespace SMS.EPA.Model
{
    using SMS.EPA.Model.Enitites;

    public class DisciplineSummaryModel
    {
        public Employee Employee { get; set; }

        public decimal ChangeShift { get; set; }

        public decimal MissingCheckInOut { get; set; }

        public decimal LateIn { get; set; }

        public decimal EarlyOut { get; set; }

        public decimal Absent { get; set; }

        public decimal UnpaidLeave { get; set; }

        public decimal WorkingUndertime { get; set; }

        public decimal UnpaidLeaveAbsent { get; set; }

        public decimal OtherDisciplines { get; set; }

        public decimal SystemSubtractScore { get; set; }

        public decimal ApproverSubtractScore
        {
            get
            {
                decimal total = ChangeShift + MissingCheckInOut + LateIn + EarlyOut + Absent + UnpaidLeave + WorkingUndertime + UnpaidLeaveAbsent + OtherDisciplines;
                return total;
            }
        }
    }
}