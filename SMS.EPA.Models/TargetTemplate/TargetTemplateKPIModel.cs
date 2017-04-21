namespace SMS.EPA.Model
{
    public class TargetTemplateKPIModel
    {
        public int KPIId { get; set; }

        public string Name { get; set; }

        public string KPIGroupName { get; set; }

        public int KPIGroupId { get; set; }

        public bool IsSpreadKPI { get; set; }

        public bool IsSpreadKPIOriginal => IsSpreadKPI;

        public double? Weight { get; set; }

        public int? DisplayOrder { get; set; }

        public bool IsRequiredEvaluate { get; set; }

        public bool HasComment { get; set; }

        public bool IsRequiredComment { get; set; }
    }
}