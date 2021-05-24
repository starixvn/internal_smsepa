namespace SMS.EPA.Criteria.JobLevel
{
    using Fanex.Data.Repository;

    public class DeleteJobLevelCriteria : CriteriaBase
    {
        public int JobLevelId { get; set; }

        public override string GetSettingKey() => "JobLevel_Delete";

        public override bool IsValid() => JobLevelId > 0 ;
    }
}