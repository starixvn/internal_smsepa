namespace SMS.EPA.Model.Discipline
{
    using System;

    public class DisciplineSummaryBase
    {
        protected decimal approverSubtractScore;

        public decimal MissingCheckInOut { get; set; }

        public decimal LateIn { get; set; }

        public decimal EarlyOut { get; set; }

        public decimal ChangeShift { get; set; }

        public decimal Absent { get; set; }

        public decimal UnpaidLeave { get; set; }

        public decimal UnplannedLeave { get; set; }

        public decimal WeekendLeave { get; set; }

        public decimal WorkingUndertime { get; set; }

        public decimal UnpaidLeaveAbsent { get; set; }

        public decimal OtherDisciplines { get; set; }

        public decimal SystemSubtractScore { get; set; }

        public decimal ApproverIgnoredScore { get; set; }

        public virtual decimal ApproverSubtractScore { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}