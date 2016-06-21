namespace SMS.EPA.Criteria.KPIGroup
{
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Attribute;

    public class CreateKPIGroupCriteria : CriteriaBase
    {
        [Output]
        public int KPIGroupId { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public string Description { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "KPIGroup_Insert";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }
    }
}