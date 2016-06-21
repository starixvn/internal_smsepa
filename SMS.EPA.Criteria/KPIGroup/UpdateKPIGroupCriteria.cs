namespace SMS.EPA.Criteria.KPIGroup
{
    using Fanex.Data.Repository;

    public class UpdateKPIGroupCriteria : CriteriaBase
    {
        public int KPIGroupId { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public string Description { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "KPIGroup_Update";
        }

        public override bool IsValid()
        {
            return KPIGroupId > 0 && !string.IsNullOrWhiteSpace(Name);
        }
    }
}