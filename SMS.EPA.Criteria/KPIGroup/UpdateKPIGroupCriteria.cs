namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateKPIGroupCriteria : CriteriaBase
    {
        public int KPIGroupId { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public string Description { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "KPIGroup_Update";

        public override bool IsValid() => KPIGroupId > 0 && !string.IsNullOrWhiteSpace(Name) && ModifiedBy > 0;
    }
}