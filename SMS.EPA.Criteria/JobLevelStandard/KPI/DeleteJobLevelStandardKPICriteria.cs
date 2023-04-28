namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class DeleteJobLevelStandardKPICriteria : CriteriaBase
    {
        public int KPIId { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid() => KPIId > 0 && ModifiedBy > 0;

        public override string GetSettingKey() => "JobLevelStandard_KPI_Delete";
    }
}