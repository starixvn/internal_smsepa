namespace SMS.EPA.Criteria.JobLevelStandard
{
    using Fanex.Data.Repository;

    public class GetPeriodJobLevelSettingSummaryCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }
        public bool? HasSetting { get; set; }

        public override string GetSettingKey() => "JobLevelStandard_PeriodJobLevelSetting_Get";

        public override bool IsValid() => true;
    }
}