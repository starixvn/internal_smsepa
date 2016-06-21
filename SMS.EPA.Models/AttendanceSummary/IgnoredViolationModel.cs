namespace SMS.EPA.Model.AttendanceSummary
{
    using System;

    public class IgnoredViolationModel
    {
        public DateTime WorkDate { get; set; }

        public int EmployeeId { get; set; }

        public int ViolationId { get; set; }

        public bool Ignored { get; set; }
    }
}