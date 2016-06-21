namespace SMS.EPA.Criteria.KPI
{
    using Fanex.Data.Repository;

    public class DeleteKPICriteria : CriteriaBase
    {
        public int KPIId { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return KPIId > 0;
        }

        public override string GetSettingKey()
        {
            return "KPI_Delete";
        }
    }
}