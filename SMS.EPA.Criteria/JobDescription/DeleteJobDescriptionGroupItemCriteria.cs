namespace SMS.EPA.Criteria.JobDescription
{
    using Fanex.Data.Repository;

    public class DeleteJobDescriptionGroupItemCriteria : CriteriaBase
    {
        public int GroupItemId { get; set; }

        public override string GetSettingKey() => "JobDescription_Delete";

        public override bool IsValid() => GroupItemId > 0 ;
    }
}