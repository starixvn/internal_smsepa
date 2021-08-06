namespace SMS.EPA.Criteria.JobDescription
{
    using Fanex.Data.Repository;

    public class CopyGroupSettingJobDescriptionCriteria : CriteriaBase
    {
        public int FromGroupId { get; set; }
        public int ToGroupId { get; set; }
        public int ActorId { get; set; }
        public int? JobLevelId { get; set; }

        public override string GetSettingKey() => "JobDescription_CopyGroupSetting";

        public override bool IsValid() => FromGroupId > 0 && ToGroupId > 0 && ActorId > 0;
    }
}