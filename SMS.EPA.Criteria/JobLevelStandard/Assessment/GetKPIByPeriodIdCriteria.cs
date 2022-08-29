namespace SMS.EPA.Criteria.JobLevelStandard
{
    using Fanex.Data.Repository;

    public class GetKPIByPeriodIdCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public override string GetSettingKey() => "JobLevelStandard_Assessment_GetKPIByPeriodId";

        public override bool IsValid() => PeriodId > 0;
    }
}