namespace SMS.EPA.Criteria.TargetType
{
    using Fanex.Data.Repository;

    public class GetTargetTypeCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public string ExcludeTargetTypeIds { get; set; }

        public override string GetSettingKey()
        {
            return "TargetType_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}