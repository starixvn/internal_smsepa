namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateStatusKPICriteria : CriteriaBase
    {
        public int KPIId { get; set; }

        public bool IsActive { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "KPI_UpdateStatus";

        public override bool IsValid() => KPIId > 0 && ModifiedBy > 0;
    }
}