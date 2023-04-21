namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateStatusJobLevelStandardKPICriteria : CriteriaBase
    {
        public int KPIId { get; set; }

        public bool IsActive { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "JobLevelStandard_KPI_UpdateStatus";

        public override bool IsValid() => KPIId > 0 && ModifiedBy > 0;
    }
}