namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateStatusKPIGroupCriteria : CriteriaBase
    {
        public int KPIGroupId { get; set; }

        public bool IsActive { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "KPIGroup_UpdateStatus";

        public override bool IsValid() => KPIGroupId > 0 && ModifiedBy > 0;
    }
}