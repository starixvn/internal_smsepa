namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateRequiredEvaluateKPICriteria : CriteriaBase
    {
        public int KPIId { get; set; }

        public bool IsRequiredEvaluate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "KPI_UpdateRequiredEvaluate";

        public override bool IsValid() => KPIId > 0 && ModifiedBy > 0;
    }
}