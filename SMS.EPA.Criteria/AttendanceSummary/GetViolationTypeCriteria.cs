namespace SMS.EPA.Criteria.AttendanceSummary
{
    using Fanex.Data.Repository;

    public class GetViolationTypeCriteria : CriteriaBase
    {
        public bool IncludeOtherViolate { get; set; }

        public bool? IsActive { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaAttendanceSummary_GetViolationType";
        }
    }
}