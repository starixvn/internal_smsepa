namespace SMS.EPA.Criteria.JobLevelStandard
{
    using Fanex.Data.Repository;

    public class CopyDefaultJobLevelSettingCommand : CriteriaBase
    {
        public int PeriodId { get; set; }
        public string SectionIds { get; set; }
        public int ModifiedBy { get; set; }

        public override bool IsValid() => PeriodId > 0 && !string.IsNullOrWhiteSpace(SectionIds) && ModifiedBy > 0;

        public override string GetSettingKey() => "JobLevelStandard_PeriodJobLevelSetting_CopyFromDefault";
    }
}