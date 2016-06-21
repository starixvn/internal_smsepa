namespace SMS.EPA.Criteria.Period
{
    using Fanex.Data.Repository;

    public class GetProcessSettingPeriodCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaPeriod_GetProcessSetting";
        }
    }
}