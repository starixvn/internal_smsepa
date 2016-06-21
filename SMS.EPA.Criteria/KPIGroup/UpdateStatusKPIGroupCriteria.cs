namespace SMS.EPA.Criteria.KPIGroup
{
    using Fanex.Data.Repository;

    public class UpdateStatusKPIGroupCriteria : CriteriaBase
    {
        public int KPIGroupId { get; set; }

        public bool IsActive { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "KPIGroup_UpdateStatus";
        }

        public override bool IsValid()
        {
            return KPIGroupId > 0;
        }
    }
}