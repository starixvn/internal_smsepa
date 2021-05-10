namespace SMS.EPA.Criteria.JobDescription
{
    using Fanex.Data.Repository;

    public class UpdateStatusDescriptionItemCriteria : CriteriaBase
    {
        public int ItemId { get; set; }

        public bool IsActive { get; set; }

        public override string GetSettingKey() => "JobDescription_UpdateStatusItem";

        public override bool IsValid() => ItemId > 0;
    }
}