namespace SMS.EPA.Criteria.JobLevelStandard.Report
{
    using Fanex.Data.Repository;

    public class GetPeriodCriteria : CriteriaBase
    {
        public override bool IsValid() => true;

        public override string GetSettingKey() => "JobLevelStandard_Report_GetPeriod";
    }
}