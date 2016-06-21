namespace SMS.EPA.Criteria.AttendanceSummary
{
    using System;
    using Fanex.Data.Repository;

    public class GetOtherViolationsOfEmployeeCriteria : CriteriaBase
    {
        public string EmployeeIds { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime? WorkDate { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(EmployeeIds) && StartDate <= EndDate;
        }

        public override string GetSettingKey()
        {
            return "EpaAttendanceSummary_GetOtherViolations";
        }
    }
}