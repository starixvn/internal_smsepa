namespace SMS.EPA.Criteria.AttendanceSummary
{
    using Fanex.Data.Repository;

    public class GetOtherViolationTypeCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "OtherViolationType_Get";
        }
    }
}