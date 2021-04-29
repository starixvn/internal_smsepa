namespace SMS.EPA.Criteria.JobDescription
{
    using Fanex.Data.Repository;

    public class UpdateJobDescriptionGroupItemCriteria : CriteriaBase
    {
        public int GroupItemId { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public override string GetSettingKey() => "JobDescription_UpdateGroupItem";

        public override bool IsValid() => GroupItemId > 0 && !string.IsNullOrWhiteSpace(Name);
    }
}