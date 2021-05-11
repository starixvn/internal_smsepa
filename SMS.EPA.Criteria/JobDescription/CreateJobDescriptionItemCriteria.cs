namespace SMS.EPA.Criteria.JobDescription
{
    using Fanex.Data.Repository;

    public class CreateJobDescriptionItemCriteria : CriteriaBase
    {
        public int GroupItemId { get; set; }
        public string Name { get; set; }

        public override string GetSettingKey() => "JobDescription_InsertItem";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Name) && GroupItemId > 0;
    }
}