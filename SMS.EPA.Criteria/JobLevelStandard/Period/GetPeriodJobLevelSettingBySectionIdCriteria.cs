namespace SMS.EPA.Criteria.JobLevelStandard
{
    using Fanex.Data.Repository;

    public class GetPeriodJobLevelSettingBySectionIdCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }
        public int SectionId { get; set; }

        public override string GetSettingKey() => "JobLevelStandard_PeriodJobLevelSetting_GetBySectionId";

        public override bool IsValid() => true;
    }
}