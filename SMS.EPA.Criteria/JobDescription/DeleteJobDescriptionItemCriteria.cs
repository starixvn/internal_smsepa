namespace SMS.EPA.Criteria.JobDescription
{
    using Fanex.Data.Repository;

    public class DeleteJobDescriptionItemCriteria : CriteriaBase
    {
        public int ItemId { get; set; }

        public override string GetSettingKey() => "JobDescription_DeleteItem";

        public override bool IsValid() => ItemId > 0 ;
    }
}