namespace SMS.EPA.Criteria.JobDescription
{
    using Fanex.Data.Repository;

    public class GetGroupSettingJobDescriptionByIdCriteria : CriteriaBase
    {
        public int GroupId { get; set; }
        public int JobLevelId { get; set; }

        public override bool IsValid() => GroupId > 0;

        public override string GetSettingKey() => "JobDescription_GetGroupSettingById";
    }
}