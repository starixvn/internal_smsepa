namespace SMS.EPA.Criteria.JobLevelStandard
{
    using Fanex.Data.Repository;

    public class GetJobLevelSettingSummaryCriteria : CriteriaBase
    {
        public bool? HasSetting { get; set; }

        public override string GetSettingKey() => "JobLevelStandard_JobLevelSetting_Get";

        public override bool IsValid() => true;
    }
}