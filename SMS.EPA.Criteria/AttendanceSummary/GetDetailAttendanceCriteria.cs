namespace SMS.EPA.Criteria.AttendanceSummary
{
    using System;
    using Fanex.Data.Repository;

    public class GetDetailAttendanceCriteria : CriteriaBase
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int? EmployeeId { get; set; }

        public int? DepartmentId { get; set; }

        public bool? IsLocal { get; set; }

        public int? ReasonId { get; set; }

        public int ApproverId { get; set; }

        public bool? HasIgnored { get; set; }

        public bool ShowAll { get; set; }

        [SpParam(Ignored = true)]
        public string EmployeeName { get; set; }

        [SpParam(Ignored = true)]
        public string DepartmentName { get; set; }

        [SpParam(Ignored = true)]
        public bool HasChanged { get; set; }

        public override bool IsValid()
        {
            return StartDate <= EndDate && ApproverId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaAttendanceSummary_GetDetail";
        }
    }
}