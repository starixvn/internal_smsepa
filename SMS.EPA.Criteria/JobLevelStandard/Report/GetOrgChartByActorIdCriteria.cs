namespace SMS.EPA.Criteria.JobLevelStandard.Report
{
    using Fanex.Data.Repository;

    public class GetOrgChartByActorIdCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public int? PeriodId { get; set; }

        public override bool IsValid() => ActorId > 0;

        public override string GetSettingKey() => "JobLevelStandard_Report_GetOrgChartByActorId";
    }
}