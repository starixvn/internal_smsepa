namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Attribute;

    public class CreateKPICriteria : CriteriaBase
    {
        [Output]
        public int KPIId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string KPIGroupIds { get; set; }

        public bool HasComment { get; set; }

        public bool IsRequiredComment { get; set; }

        public bool IsRequiredEvaluate { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "KPI_Insert";
        }

        public override bool IsValid()
        {
            return CreatedBy > 0 && !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(KPIGroupIds);
        }
    }
}