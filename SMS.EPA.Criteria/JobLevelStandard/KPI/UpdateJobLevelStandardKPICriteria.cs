namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateJobLevelStandardKPICriteria : CriteriaBase
    {
        public int KPIId { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "JobLevelStandard_KPI_Update";

        public override bool IsValid() => KPIId > 0 && ModifiedBy > 0 && !string.IsNullOrWhiteSpace(Name);
    }
}