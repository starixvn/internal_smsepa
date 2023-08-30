namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetUnitCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "Unit_Get";
    }
}