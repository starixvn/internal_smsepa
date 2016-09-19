namespace SMS.EPA.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetOtherViolationsOfEmployeeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime? WorkDate { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0 && StartDate <= EndDate;
        }

        public override string GetSettingKey()
        {
            return "EpaAttendanceSummary_GetOtherViolations";
        }
    }
}