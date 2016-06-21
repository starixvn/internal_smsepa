namespace SMS.EPA.Criteria.KPI
{
    using Fanex.Data.Repository;

    public class GetKPICriteria : CriteriaBase
    {
        public int? KPIGroupId { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsRequiredEvaluate { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "KPI_Get";
        }
    }
}