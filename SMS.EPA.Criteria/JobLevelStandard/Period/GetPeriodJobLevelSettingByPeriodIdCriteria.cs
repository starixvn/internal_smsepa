namespace SMS.EPA.Criteria.JobLevelStandard
{
    using Fanex.Data.Repository;

    public class GetPeriodJobLevelSettingByPeriodIdCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public override string GetSettingKey() => "JobLevelStandard_PeriodJobLevelSetting_GetByPeriodId";

        public override bool IsValid() => PeriodId > 0;
    }
}