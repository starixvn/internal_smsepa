namespace SMS.EPA.Criteria.JobDescription
{
    using Fanex.Data.Repository;

    public class UpdateStatusDescriptionGroupItemCriteria : CriteriaBase
    {
        public int GroupItemId { get; set; }

        public bool IsActive { get; set; }

        public override string GetSettingKey() => "JobDescription_UpdateStatusGroupItem";

        public override bool IsValid() => GroupItemId > 0;
    }
}