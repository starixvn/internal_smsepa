namespace SMS.EPA.Criteria.AttendanceSummary
{
    using System;
    using Fanex.Data.Repository;

    public class InsertOtherViolationsCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public DateTime WorkDate { get; set; }

        public string Name { get; set; }

        public float Score { get; set; }

        public string Comment { get; set; }

        public int CreatedBy { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0 && !string.IsNullOrWhiteSpace(Name);
        }

        public override string GetSettingKey()
        {
            return "EpaAttendanceSummary_AddOtherViolations";
        }
    }
}