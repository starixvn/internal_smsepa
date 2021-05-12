namespace SMS.EPA.Criteria.JobDescription
{
    using Fanex.Data.Repository;

    public class EditJobDescriptionItemCriteria : CriteriaBase
    {
        public int GroupItemId { get; set; }
        public int ItemId { get; set; }
        public string Name { get; set; }

        public override string GetSettingKey() => "JobDescription_UpdateItem";

        public override bool IsValid() => GroupItemId > 0 && ItemId > 0 && !string.IsNullOrWhiteSpace(Name);
    }
}