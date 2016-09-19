namespace SMS.EPA.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetAttendanceSummaryCriteria : CriteriaBase
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int ApproverId { get; set; }

        public int? DepartmentId { get; set; }

        public bool? IsLocal { get; set; }

        public int? EmployeeId { get; set; }

        public int? ReasonId { get; set; }

        [SpParam(Ignored = true)]
        public string EmployeeName { get; set; }

        [SpParam(Ignored = true)]
        public string DepartmentName { get; set; }

        public override bool IsValid()
        {
            return EndDate >= StartDate && ApproverId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaAttendanceSummary_Get";
        }
    }
}