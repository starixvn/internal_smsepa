namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetJobDescriptionByEPAIdCriteria : CriteriaBase
    {
        public int EPAId { get; set; }

        public override string GetSettingKey() => "EPA_GetJobDescriptiontionById";

        public override bool IsValid() => EPAId > 0;
    }
}