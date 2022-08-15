namespace SMS.EPA.Criteria.JobLevelStandard
{
    using Fanex.Data.Repository;

    public class GetJobLevelSettingBySectionIdCriteria : CriteriaBase
    {
        public int SectionId { get; set; }

        public override string GetSettingKey() => "JobLevelStandard_JobLevelSetting_GetBySectionId";

        public override bool IsValid() => true;
    }
}