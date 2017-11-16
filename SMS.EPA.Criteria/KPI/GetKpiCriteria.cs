namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetKPICriteria : CriteriaBase
    {
        public int? KPIGroupId { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsRequiredEvaluate { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "KPI_Get";
    }
}