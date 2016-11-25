namespace SMS.EPA.ClientHrm.Model
{
    using System;

    public class DisciplineSummary
    {
        public int EmployeeId { get; set; }

        public DateTime Period { get; set; }

        public decimal MissingCheckInOut { get; set; }

        public decimal LateIn { get; set; }

        public decimal EarlyOut { get; set; }

        public decimal ChangeShift { get; set; }

        public decimal Absent { get; set; }

        public decimal UnpaidLeave { get; set; }

        public decimal WorkingUndertime { get; set; }

        public decimal UnpaidLeaveAbsent { get; set; }

        public decimal OtherDisciplines { get; set; }

        public decimal SystemSubtractScore { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}