namespace SMS.EPA.Criteria.JobLevelStandard.KPI
{
    using Fanex.Data.Repository;
    public class GetJobLevelStandardKPICriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "JobLevelStandard_KPI_Get";
    }
}
