namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetDisciplineTypeCriteria : CriteriaBase
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