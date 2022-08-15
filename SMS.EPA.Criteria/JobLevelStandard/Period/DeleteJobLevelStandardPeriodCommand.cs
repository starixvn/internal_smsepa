namespace SMS.EPA.Criteria.JobLevelStandard
{
    using Fanex.Data.Repository;

    public class DeleteJobLevelStandardPeriodCommand : CriteriaBase
    {
        public int PeriodId { get; set; }
        public int ModifiedBy { get; set; }

        public override bool IsValid() => PeriodId > 0 && ModifiedBy > 0;

        public override string GetSettingKey() => "JobLevelStandard_Period_Delete";
    }
}