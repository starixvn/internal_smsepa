namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class CreateKPIGroupCriteria : CriteriaBase
    {
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
            return !string.IsNullOrWhiteSpace(Name) && CreatedBy > 0;
        }
    }
}