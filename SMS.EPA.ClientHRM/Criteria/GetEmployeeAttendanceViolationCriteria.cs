namespace SMS.EPA.ClientHRM.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetEmployeeAttendanceViolationCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeAttendanceViolation_Get";
        }
    }
}
