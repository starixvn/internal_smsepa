namespace SMS.EPA.Criteria.JobDescription
{
    using Fanex.Data.Repository;

    public class UpdateJobDescriptionItemCriteria : CriteriaBase
    {
        public int ItemId { get; set; }

        public string Name { get; set; }

        public override string GetSettingKey() => "JobDescription_UpdateItem";

        public override bool IsValid() => ItemId > 0 && !string.IsNullOrWhiteSpace(Name);
    }
}